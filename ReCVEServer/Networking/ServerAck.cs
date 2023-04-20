namespace ReCVEServer.Networking {
    public class ServerAck {

        public string type { get; private set; }
        //type = "serverAck";
        public int id { get; set; }
        public ServerAck() {
            type = "serverAck";
        }

    }
}
