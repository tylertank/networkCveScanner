namespace ReCVEServer.Networking.Events {
    public class AssemblyEvent {

        public int ID { get; set; }
        public string AssemblyName { get; set; }
        public string AssemblyData { get; set; }
        public List<string> args { get; set; }

        public AssemblyEvent( int iD,string assemblyName, string assemblyData, List<string> arguments) {
            ID = iD;
            AssemblyName = assemblyName;
            AssemblyData = assemblyData;
            args = arguments;
        }
    }
}
