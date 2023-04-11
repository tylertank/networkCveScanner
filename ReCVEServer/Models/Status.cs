namespace ReCVEServer.Models
{
    public class Status
    {
        public int ID { get; set; }
        public int clientID { get; set; }
        public string processStatus { get; set; }

        public float cpu { get; set; }

        public float memory { get; set; }


    }
}
