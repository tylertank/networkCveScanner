namespace ReCVEServer.Models
{
    public class Software
    {
        public int ID { get; set; } 
        public Client client { get; set; }
        public string? vulnerable { get; set; } 
        public string? details { get; set; }
        public ICollection<Vulnerability> Vulnerabilities { get; set; }
    }
}
