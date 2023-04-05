namespace ReCVEServer.Models
{
    public class CVE
    {
        public int ID { get; set; }
        public string? Name { get; set; }
        public string? IPAddress { get; set; }
        public DateTime EnrollmentDate { get; set; }
    }
}
