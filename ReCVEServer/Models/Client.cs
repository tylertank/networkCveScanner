namespace ReCVEServer.Models
{
    public class Client
    {
        public int ID { get; set; }
        public string? Name { get; set; }
        public string? IPAddress { get; set; }
        
        public string? OS { get; set; }

        public string? OSVersion { get; set; }
        public DateTime EnrollmentDate { get; set; }
        

    }
}
