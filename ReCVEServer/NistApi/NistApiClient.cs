using Microsoft.Extensions.Configuration;

namespace ReCVEServer.NistApi
{
    public class NistApiClient
    {

        private readonly NistApiConfig _config;

        public NistApiClient(NistApiConfig config)
        {
            _config = config;
        }

        public async Task<string> GetCveDataAsync(int startIndex, int resultsPerPage)
        {
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("apiKey", _config.ApiKey);

            var requestUrl = $"{_config.BaseUrl}:o:Microsoft:Windows_10:20H2&startIndex={startIndex}&resultsPerPage={resultsPerPage}";
            var response = await client.GetAsync(requestUrl);

            if (!response.IsSuccessStatusCode)
            {
           
                throw new Exception($"Request failed: "); //{message}
            }

            return await response.Content.ReadAsStringAsync();
        }
    }

    public class NistApiConfig
    {
        public string BaseUrl { get; set; }
        public string ApiKey { get; set; }

        public NistApiConfig(IConfiguration configuration)
        {
            BaseUrl = "https://services.nvd.nist.gov/rest/json/cpes/2.0?cpeMatchString=cpe:2.3"; // Replace with the actual API URL
            ApiKey = configuration.GetValue<string>("NistApiConfig:ApiKey");
        }
    }

}
