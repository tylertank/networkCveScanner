namespace ReCVEServer.Networking {
    public class ServerCommand {
        public string type { get; private set; }
       
        public string command { get; set; }
        public int frequency { get; set; }
        public ServerCommand() {
            type = "serverCommand";
        }
    }
}
