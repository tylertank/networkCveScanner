namespace ReCVEServer.Models
{
    public class CVE
    {
        public int ID { get; set; }
        public string cveID { get; set; }
        public string? description { get; set; }
        public DateTime published { get; set; }
        public double baseScore { get; set; }
    }
}
