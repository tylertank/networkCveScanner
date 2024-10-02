namespace ReCVEServer.Networking.ClientProcessing {
    public class AssemblyCommand {
        public string type { get; private set; }
        public string? name { get; private set; }
        public string? data { get; private set; }
        public List<string>? args { get; private set; }
        public AssemblyCommand() {
            type = "assemblyCommand";
        }
        public AssemblyCommand(string type, string name, string data, List<string> args) {
            this.type = type;
            this.name = name;
            this.data = data;
            this.args = args;
        }
    }
}

