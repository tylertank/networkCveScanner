namespace ReCVEServer.Models {
    public class ClientCVEViewModel {
      
            public IEnumerable<ReCVEServer.Models.Client> Clients { get; set; }
            public IEnumerable<dynamic> CVEs { get; set; }
            public IEnumerable<ReCVEServer.Models.CveHistory> History { get; set; }



    }
}
