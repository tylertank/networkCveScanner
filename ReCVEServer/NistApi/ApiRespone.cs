using Newtonsoft.Json;

namespace ReCVEServer.NistApi
{
    public class ApiResponse
    {
        [JsonProperty("vulnerabilities")]
        public List<Cve> vulnerabilities { get; set; }
        [JsonProperty("resultsPerPage")]
        public int resultsPerPage { get; set; }
        [JsonProperty("startIndex")]
        public int startIndex { get; set; }
        [JsonProperty("totalResults")]
        public int totalResults { get; set; }
       

    }

    public class Cve
    {
        [JsonProperty("cve")]
        public CveData Data { get; set; }
    }
    public class CveData
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("published")]
        public DateTime Published { get; set; }

        [JsonProperty("descriptions")]
        public List<Description> Descriptions { get; set; }

        [JsonProperty("metrics")]
        public Metrics Metrics { get; set; }
    }
    public class Metrics
    {
        [JsonProperty("cvssMetricV30")]
        public List<CvssMetricV3> CvssMetricV3List { get; set; }
    }

    public class CvssMetricV3
    {
        [JsonProperty("cvssData")]
        public CvssData CvssData { get; set; }
    }

    public class CvssData
    {
        [JsonProperty("baseScore")]
        public double BaseScore { get; set; }
    }
    public class Description
    {
        public string Lang { get; set; }
        public string Value { get; set; }
    }

}
