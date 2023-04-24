using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NuGet.Packaging;
using NuGet.Protocol.Plugins;
using ReCVEServer.Data;
using ReCVEServer.Models;
using System;
using System.Diagnostics.SymbolStore;
using static System.Net.Mime.MediaTypeNames;
using System.Numerics;
using static System.Net.WebRequestMethods;

namespace ReCVEServer.NistApi
{
    public class NistApiClient
    {
        private readonly ReCVEServerContext _context;
        private readonly NistApiConfig _config;

        public NistApiClient(NistApiConfig config, ReCVEServerContext context)
        {
            _config = config;
            _context = context;
        }

        /// <summary>
        /// Checks the software version against the NIST API and updates the CVE database with any new vulnerabilities.
        /// </summary>
        public async Task checkSoftware() {
         
            List<Software> allSoftware = _context.Softwares.ToList();
            // Create a HashSet to store unique combinations of vendor, application, and version strings
            HashSet<string> uniqueCombinations = new HashSet<string>();

            foreach (Software software in allSoftware){
                string combination = $"{software.vendor}-{software.application}-{software.version}";
                // If the combination is not already in the HashSet, add it
                if (!uniqueCombinations.Contains(combination)){
                    uniqueCombinations.Add(combination);
                }
            }

           bool result = await sendReq(uniqueCombinations);
            
           
        }


        /// <summary>
        /// Sends a request to the NIST API with the specified software versions and updates the CVE database with any new vulnerabilities.
        /// </summary>
        /// <param name="softwares">The set of software versions to check.</param>
        /// <returns>True if the operation succeeded, false otherwise.</returns>
        public async Task<bool> sendReq(HashSet<string> softwares)
        {
            try
            {
                var existingCVEs = await _context.CVEs.ToListAsync();
                // Loop through all software combinations in the HashSet
                foreach (var s in softwares)
                {
                   string test = s.Replace(' ', '_');
                    var software = test.Split('-');

                    string vendor = software[0];
                    string application = software[1];
                    string version = software[2];
                CVE tempCVE;

                    // Fetch all vulnerabilities for the vendor, application, and version from the NIST API
                    var allVulnerabilities = await FetchAllVulnerabilitiesAsync("a", application, vendor, version);
                    // Filter out any vulnerabilities that are already in the database
                    var newVulnerabilities = allVulnerabilities.Where(v => !existingCVEs.Any(e => e.cveID == v.Data.Id && e.vendor == vendor && e.application == application && e.version == version));

                    foreach (var vulnerability in newVulnerabilities)
                {
                   
                    tempCVE = new CVE();
                    tempCVE.cveID = vulnerability.Data.Id;
                    tempCVE.published = vulnerability.Data.Published;
                    tempCVE.vendor = vendor;
                    tempCVE.version = version;
                    tempCVE.application = application;

                    if (vulnerability.Data.Metrics.CvssMetricV3List != null)
                    {
                        tempCVE.baseSeverity = vulnerability.Data.Metrics.CvssMetricV3List.First().CvssData.Severity;
                        tempCVE.baseScore = vulnerability.Data.Metrics.CvssMetricV3List.First().CvssData.BaseScore;
                    }
                    else if (vulnerability.Data.Metrics.CvssMetricV31List != null)
                    {
                        tempCVE.baseSeverity = vulnerability.Data.Metrics.CvssMetricV31List.First().CvssData.Severity;
                        tempCVE.baseScore = vulnerability.Data.Metrics.CvssMetricV31List.First().CvssData.BaseScore;
                    }
                    else if (vulnerability.Data.Metrics.CvssMetricV2List != null)
                    {
                        tempCVE.baseSeverity = vulnerability.Data.Metrics.CvssMetricV2List.First().Severity;
                        tempCVE.baseScore = vulnerability.Data.Metrics.CvssMetricV2List.First().CvssData.BaseScore;
                    }
                    else
                    {

                        tempCVE.baseScore = -1.0;
                        tempCVE.baseSeverity = "notFound";
                    }
                    tempCVE.description = vulnerability.Data.Descriptions.First().Value;
                    _context.CVEs.Add(tempCVE);
                }

                }
                await _context.SaveChangesAsync();
             
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
       
        }
        /// <summary>
        /// Fetches all the vulnerabilities for the specified software version from the NIST API.
        /// </summary>
        /// <param name="type">The type of CPE name to use for the search (a, o, h).</param>
        /// <param name="application">The name of the application to search for.</param>
        /// <param name="vendor">The name of the vendor to search for.</param>
        /// <param name="version">The version of the software to search for.</param>
        /// <returns>A list of vulnerabilities.</returns>
        public async Task<List<NistApi.Cve>> FetchAllVulnerabilitiesAsync(string type, string application, string vendor, string version)
        {
            var allVulnerabilities = new List<NistApi.Cve>();

            // Fetch the first page
            var apiResponse = await GetCveData(type, application, vendor, version, 0);

            // Calculate the number of pages
            int totalPages = (int)Math.Ceiling((double)apiResponse.totalResults / apiResponse.resultsPerPage);

            // Add the first page's vulnerabilities to the list
            allVulnerabilities.AddRange(apiResponse.vulnerabilities);

            // Fetch the remaining pages
            if(totalPages > 1){

            for (int pageIndex = 1; pageIndex < totalPages; pageIndex++){
                apiResponse = await GetCveData(type, application, vendor, version, pageIndex);
                allVulnerabilities.AddRange(apiResponse.vulnerabilities);
            }
            }

            return allVulnerabilities;
        }

        /// <summary>
        /// Sends a request to the NIST API with the specified parameters and returns the response.
        /// </summary>
        /// <param name="type">The type of CPE name to use for the search (a, o, h).</param>
        /// <param name="application">The name of the application to search for.</param>
        /// <param name="vendor">The name of the vendor to search for.</param>
        /// <param name="version">The version of the software to search for.</param>
        /// <param name="pageIndex">The index of the page to fetch.</param>
        /// <returns>The response from the NIST API.</returns>
        public async Task<ApiResponse> GetCveData(string type, string application, string vendor, string version, int pageIndex)
        {
          
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("apiKey", _config.ApiKey);
            var requestUrl = "";
            if (vendor == "" || version == "")
            {
                if (vendor == "")
                {
                    vendor = "*";
                }
                if (version == "")
                {
                    version = "*";
                }
                requestUrl = $"{_config.BaseUrl}virtualMatchString=cpe:2.3:{type}:{vendor}:{application}:{version}&startIndex={pageIndex}";
            }
            else
            {
                requestUrl = $"{_config.BaseUrl}cpeName=cpe:2.3:{type}:{vendor}:{application}:{version}&startIndex={pageIndex}";
            }

            //var requestUrl = "https://services.nvd.nist.gov/rest/json/cves/2.0?cpeName=cpe:2.3:o:microsoft:windows_10:22h2&startIndex=0"; basic syntax for api request
             var response = await client.GetAsync(requestUrl);

            // If the response is a "Too Many Requests" status code, wait for one second and try again
            if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
            {
                await Task.Delay(1000);
            }
            else if (!response.IsSuccessStatusCode)
            {
                // if this is failing have you added the api key to user.secrets?
                throw new Exception($"Request failed: "); //{message}
            }
            string jsonResponse = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ApiResponse>(jsonResponse);
            }
    }
    /// <summary>
    /// Represents the configuration for the NIST API.
    /// </summary>
    public class NistApiConfig
    {
        public string BaseUrl { get; set; }
        public string ApiKey { get; set; }

        public NistApiConfig(IConfiguration configuration)
        {

            //https://services.nvd.nist.gov/rest/json/cves/2.0?cpeName=cpe:2.3:o:microsoft:windows_10:1607:*:*:*:*:*:*:*
            BaseUrl = "https://services.nvd.nist.gov/rest/json/cves/2.0?"; // Replace with the actual API URL
            ApiKey = configuration.GetValue<string>("NistApiConfig:ApiKey");
        }
    }

}
