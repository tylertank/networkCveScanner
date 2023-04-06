namespace ReCVEServer.Models
{
    public class Software
    {
        public int ID { get; set; } 
        public Client client { get; set; }
        public CVE CVE { get; set; }
        public string? status { get; set; } 
        public string? details { get; set; }
    }
}
