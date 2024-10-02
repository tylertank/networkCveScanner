using CommonReCVE.Crypto;
using Newtonsoft.Json;
using ReCVEServer.Networking.ClientProcessing;

namespace ReCVEServer.Networking {
    /// <summary>
    /// This class takes a single connection with a client and sends either a process or scan update to them
    /// </summary>
    public class SendUpdateClient {
        private CryptoTunnel _tunnel;
        public SendUpdateClient(CryptoTunnel tunnel) {
            _tunnel = tunnel;
        }
        /// <summary>
        /// A process update is sent to a client to request that the frequency in which the client sends process data
        /// such as cpu usage is changed to a certain interval recorded in seconds.
        /// </summary>
        public void SendProcessUpdate(int frequency) { 
            ServerCommand command = new ServerCommand("changePollingFrequency");
            command.frequency = frequency;
            string json = JsonConvert.SerializeObject(command);
            _tunnel.SecureSend(json);
            
        }
        /// <summary>
        /// A scan update is sent to the client either when immediately requested or at a certain interval set by the user
        /// </summary>
        public void SendScanUpdate() {
            ServerCommand command = new ServerCommand("scan");
            string json = JsonConvert.SerializeObject(command);
            _tunnel.SecureSend(json);
        }

        public void SendAssembly(string name, string data, List<string> args) {          
            AssemblyCommand command = new AssemblyCommand("assembly",name,data,args);
            string json = JsonConvert.SerializeObject(command); 
            _tunnel.SecureSend(json);
        }
    }
}
