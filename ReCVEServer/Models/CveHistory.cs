namespace ReCVEServer.Models
{
    public class CveHistory
    {
        public int ID { get; set; }
        public string? cveScore{ get; set; }
        public DateTime? date { get; set; }
    }
}
