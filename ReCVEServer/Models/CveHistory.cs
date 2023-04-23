namespace ReCVEServer.Models
{
    public class CveHistory
    {
        public int ID { get; set; }
        public int totalCount { get; set; }
        public double cveScore{ get; set; }
        public DateTime? date { get; set; }
        public int? highCount { get; set; }
        public int? lowCount { get; set;}
        public int? mediumCount { get; set; }
        public int? criticalCount { get; set; }
    }
}
