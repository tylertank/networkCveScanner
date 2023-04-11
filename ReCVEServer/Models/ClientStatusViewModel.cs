namespace ReCVEServer.Models {
    public class ClientStatusViewModel {
        public IEnumerable<ReCVEServer.Models.Client> Clients { get; set; }
        public IEnumerable<ReCVEServer.Models.Status> Statuses { get; set; }
    }
}
