using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using NuGet.Packaging;
using ReCVEServer.Data;
using ReCVEServer.Models;
using System.Diagnostics.SymbolStore;
using static System.Net.WebRequestMethods;

namespace ReCVEServer.NistApi
{
    public class NistApiClient
    {
        private readonly ReCVEServerContext _context;
        private readonly NistApiConfig _config;

        public NistApiClient(NistApiConfig config)
        {
            _config = config;
        }
        public async Task<List<NistApi.Cve>> FetchAllVulnerabilitiesAsync(string application, string vendor, string version)
        {
            var allVulnerabilities = new List<NistApi.Cve>();

            // Fetch the first page
            var apiResponse = await GetCveData(0, application, vendor, version);

            // Calculate the number of pages
            int totalPages = (int)Math.Ceiling((double)apiResponse.totalResults / apiResponse.resultsPerPage);

            // Add the first page's vulnerabilities to the list
            allVulnerabilities.AddRange(apiResponse.vulnerabilities);

            // Fetch the remaining pages
            if(totalPages > 1){

            for (int pageIndex = 1; pageIndex < totalPages; pageIndex++){
                apiResponse = await GetCveData(pageIndex, application, vendor, version);
                allVulnerabilities.AddRange(apiResponse.vulnerabilities);
            }
            }



            return allVulnerabilities;
        }


        public async Task<ApiResponse> GetCveData(int pageIndex, string application,string vendor, string version)
        {
            string allData = "";
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("apiKey", _config.ApiKey);

             var requestUrl = $"{_config.BaseUrl}:o:{vendor}:{application}:{version}&startIndex={pageIndex}";
            //var requestUrl = "https://services.nvd.nist.gov/rest/json/cves/2.0?cpeName=cpe:2.3:o:microsoft:windows_10:22h2&startIndex=0"; basic syntax for api request
            var response = await client.GetAsync(requestUrl);

            if (!response.IsSuccessStatusCode)
            {
            // if this is failing have you added the api key to user.secrets?
                throw new Exception($"Request failed: "); //{message}
            }
            string jsonResponse = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<ApiResponse>(jsonResponse);
        }
    }

    public class NistApiConfig
    {
        public string BaseUrl { get; set; }
        public string ApiKey { get; set; }
        //cpe:2.3:o:microsoft:windows_10_22h2
        public NistApiConfig(IConfiguration configuration)
        {
            //https://services.nvd.nist.gov/rest/json/cves/2.0?cpeName=cpe:2.3:o:microsoft:windows_10:1607:*:*:*:*:*:*:*
            BaseUrl = "https://services.nvd.nist.gov/rest/json/cves/2.0?cpeName=cpe:2.3"; // Replace with the actual API URL
            ApiKey = configuration.GetValue<string>("NistApiConfig:ApiKey");
        }
    }

}
