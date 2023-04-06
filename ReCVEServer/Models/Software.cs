namespace ReCVEServer.Models
{
    public class Software
    {
        public int ID { get; set; } 
        public Client client { get; set; }
        public string? vulnerable { get; set; } 
        public string? vendor { get; set; }

        public string? application { get; set; }

        public string? version { get; set; }
        public ICollection<Vulnerability> Vulnerabilities { get; set; }
       

    }
}
