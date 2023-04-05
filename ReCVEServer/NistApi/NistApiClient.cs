using Microsoft.Extensions.Configuration;
using ReCVEServer.Data;
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

        public async Task<string> GetCveDataAsync(int startIndex, string application,string vendor, string version)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("apiKey", _config.ApiKey);

             var requestUrl = $"{_config.BaseUrl}:o:{vendor}:{application}:{version}&startIndex={startIndex}";
            //var requestUrl = "https://services.nvd.nist.gov/rest/json/cves/2.0?cpeName=cpe:2.3:o:microsoft:windows_10:22h2&startIndex=0"; basic syntax for api request
            var response = await client.GetAsync(requestUrl);

            if (!response.IsSuccessStatusCode)
            {
            // if this is failing have you added the api key to user.secrets?
                throw new Exception($"Request failed: "); //{message}
            }

            return await response.Content.ReadAsStringAsync();
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
