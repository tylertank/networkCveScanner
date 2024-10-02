using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using ReCVEServer.Data;
using ReCVEServer.Models;
using System.Net.Sockets;
using CommonReCVE.Extensions;
using CommonReCVE.Crypto;
using System.Text;
using ReCVEServer.Controllers;
using ReCVEServer.Networking.Events;
using ReCVEServer.Networking.ScheduledScan;
using CsvHelper;
using ReCVEServer.Networking.ClientProcessing;

namespace ReCVEServer.Networking {

    /// <summary>
    /// ProcessClient is a class designed to take a client socket passed to it
    /// from the ServerAsync method in ServerLoop. After the client hello, all communication between the server
    /// and the client will take place here until the server or the client disconnects.
    /// </summary>
    public sealed class ProcessClient {
        private IServiceScopeFactory _scopeFactory;
        private readonly ILogger<ServerLoop> _logger;
        private CryptoTunnel _tunnel;
        private int ID = 0;
        private AssemblySender assemblySender;

        public ProcessClient(IServiceScopeFactory scopeFactory, ILogger<ServerLoop> logger,ScheduleScan scan, AssemblySender assemblySender) { //},CryptoTunnel cryptTunnel) {
            _logger = logger;
            _scopeFactory = scopeFactory;
            this.assemblySender = assemblySender;
            using (var scope = _scopeFactory.CreateScope()) {
                scan.RaiseScanEvent += HandleScanUpdate;
                this.assemblySender.RaiseAssemblyEvent += HandleAssembly;             
                _logger.LogInformation("ProcessClient subscribed to RaiseAssemblyEvent.");
            }
        }
       private void HandleFrequencyUpdate (object sender, FrequencyEventArgs args)
        {
            if (args.ID == ID) {
                SendUpdateClient update = new SendUpdateClient(_tunnel);
                update.SendProcessUpdate(args.Frequency);
            }
        }
        private void HandleScanUpdate(object sender, ScanEventArgs args) {
            
            if (args.ID == ID) {
                SendUpdateClient update = new SendUpdateClient(_tunnel);
                update.SendScanUpdate( );
            }
        }

        private void HandleAssembly(object sender, AssemblyEvent args) {
            if (args.ID == ID) {               
                SendUpdateClient update = new SendUpdateClient(_tunnel);
                update.SendAssembly(args.AssemblyName, args.AssemblyData, args.args);
            }

        }

        /// <summary>
        ///  when the server recieves a json from the client it'll be directed here to be parsed
        ///  depending on the type of json it'll be sent to the appropiate function to have the 
        ///  information extracted
        /// </summary>
        /// test different types of disconnects force connect, and in the middle to writing something
        public async void directClient(TcpClient handler) {
            int id = 0;
            try
            {
                NetworkStream stream = handler.GetStream();

                _tunnel = new(stream);
                // _tunnel.Disable();
                ServerLoop.TryLoadKeys(_tunnel);
                await _tunnel.EstablishTunnelAsServer();

                string jsonS = await _tunnel.SecureReceive();

                var jResults = JObject.Parse(jsonS);
                //checks to make sure the clientHello is the first the client sent upon connecting
                if (jResults.Value<string>("type") == "clientHello") {
                    (string,int) json = await startHandshake(jResults);
                    _tunnel.SecureSend(json.Item1);
                    ServerCommand command = new ServerCommand("scan");
                   // command.command = "scan";
                    string json2 = JsonConvert.SerializeObject(command);
                    _tunnel.SecureSend(json2);
                    id = json.Item2;
                }
                else {
                    //throw error
                }
                //All additional information after the client hello is recieved and processed here
                while (handler.Connected) {
                    string jsonStr = await _tunnel.SecureReceive();
                    var jsonResults = JObject.Parse(jsonStr);
                    if (jsonResults.Value<string>("type") == "scan") {
                        await processScan(jsonResults);
                    }
                    else if (jsonResults.Value<string>("type") == "process") {
                        await processStatus(jsonResults);
                    }
                }
                handler.Close();
            }
            //Add more specific error handling so errors due to client disconnection and other errors are handled differently
            catch (IOException systEx) {
                _logger.LogInformation($"Client {id} disconnected: {systEx.Message}");
                disconClient(id, handler);
            }
            catch (Exception ex) {
                _logger.LogInformation($"{ex} was thrown. Error in process client");
                handler.Close();
             }
        }

        /// <summary>
        /// This code checks if the client is a returning client or a first time client. 
        /// </summary>
        /// <param name="jResults"></param>
        /// <returns></returns>
        private async Task<(string, int)> startHandshake(JObject jResults) {
            ServerAck serverAck = new ServerAck();
            int clientid;
            //check if this is a returning client that isn't in the database
            
                //If this is the first time a client is connecting go here
                if (jResults.Value<int>("id") == 0 || nonExistentClient(jResults.Value<int>("id"))) {
                Task<int> clientID = clientHandshake(jResults);
                clientID.Wait();
                clientid = clientID.Result;
                serverAck.id = clientID.Result;
                ID = clientID.Result;
            }
            //If the client is just reconnecting go here
            //create my own method so I don't manually try catch every .connected
            else {
                using (var scope = _scopeFactory.CreateScope()) {
                    ReCVEServerContext _context = scope.ServiceProvider.GetService<ReCVEServerContext>();
                    int id = jResults.Value<int>("id");
                    serverAck.id = id;
                    clientid = id;
                    ID= id;
                    var temp = _context.Clients.Where(c => c.ID == id).FirstOrDefault();
                    temp.online = true;
                    _context.Clients.Update(temp);
                    await _context.SaveChangesAsync();
                }
            }
            return (JsonConvert.SerializeObject(serverAck), clientid);
        }

        /// <summary>
        ///  When a client connects for the first time it'll send a client handshake json
        ///  this'll give the server the needed information to identify the client in the database
        ///  the server will also send an ack json to the client so they know what their client id is 
        ///  going forward
        /// </summary>
        private async Task<int> clientHandshake(JObject jResults) {
            System.Diagnostics.Debug.WriteLine("made it to client handshake");
            using (var scope = _scopeFactory.CreateScope()) {
                ReCVEServerContext _context = scope.ServiceProvider.GetService<ReCVEServerContext>();
                //Extract the data from the json
                var temp = jResults.GetValue("info");
                var computer = temp.Value<string>("computer");
                var ip = temp.Value<string>("ip");
                var os = temp.Value<string>("platform");
                var osVersion = temp.Value<string>("version"); 

                //parse the data into a client object
                Client client = new Client();
                client.IPAddress = ip;
                client.Name = computer;
                client.OS = os;
                client.OSVersion = osVersion;
                client.EnrollmentDate = DateTime.Now;
                client.online = true;
                client.GroupID = 1;

                //add the client object to the database  and save it
                _context.Clients.Add(client);
                await _context.SaveChangesAsync();
                ID=client.ID;
                return client.ID;
            }
        }

        /// <summary>
        ///  When the client sends a scan of their machine it'll be processed here
        /// </summary>
        private async Task processScan(JObject jResults) {
            using (var scope = _scopeFactory.CreateScope()) {
                ReCVEServerContext _context = scope.ServiceProvider.GetService<ReCVEServerContext>();
                System.Diagnostics.Debug.WriteLine("made it to process scan");
                int id = jResults.Value<int>("id");
                var objects = jResults.GetValue("objects");
                for (int i = 0; i < objects.Count(); i++) {
                    Software tempSoftware = new Software();
                    var current = objects[i];
                    tempSoftware.vendor = current.Value<string>("vendor"); //TODO remove this when client sends sanitized strings.
					string cleaned = current.Value<string>("application").Replace("\0", string.Empty);                 
					tempSoftware.application = cleaned;
                    tempSoftware.version = current.Value<string>("version");
                    tempSoftware.clientID = id;
                    if (!_context.Softwares.Any(s => s.application == tempSoftware.application && s.version == tempSoftware.version && s.clientID == tempSoftware.clientID)) {
                        _context.Softwares.Add(tempSoftware);
                        await _context.SaveChangesAsync();
                    }
                }
               
            }
        }
        /// <summary>
        ///  When the client sends an updated status think task manager type information
        ///  it'll be processed here
        /// </summary>
        private async Task processStatus(JObject jResults) {
            using (var scope = _scopeFactory.CreateScope()) {
                ReCVEServerContext _context = scope.ServiceProvider.GetService<ReCVEServerContext>();
                System.Diagnostics.Debug.WriteLine("made it to process status");
                int id = jResults.Value<int>("id");
                JToken objects = jResults.GetValue("objects");

                Status tempStatus = new Status();
                tempStatus.clientID = id;
                tempStatus.timestamp = new DateTimeOffset(DateTime.Now).ToUniversalTime().ToUnixTimeMilliseconds(); //The client should actually provide this
                tempStatus.data = objects.ToString(Formatting.None);
                tempStatus.totalCPU = jResults.Value<float>("totalCPU");
                tempStatus.totalMemory = jResults.Value<float>("totalMem");

                _context.Statuses.Add(tempStatus);

                await _context.SaveChangesAsync();
            }
        }
        /// <summary>
        /// Anytime a disconnection error is caught it will be sent to this method to properly disconnect.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="handler"></param>
        private async void disconClient(int id, TcpClient handler) {
           
            if (handler.Connected) {
                handler.Close();
            }
            // If client is disconnected before an ID is assigned, just return
            if (id == 0) {
                return;
            }
            using (var scope = _scopeFactory.CreateScope()) {
                ReCVEServerContext _context = scope.ServiceProvider.GetService<ReCVEServerContext>();
                var temp = _context.Clients.Where(c => c.ID == id).FirstOrDefault();
                temp.online = false;
                _context.Clients.Update(temp);
                await _context.SaveChangesAsync();
            }

        }
        private bool nonExistentClient(int id) {
            using (var scope = _scopeFactory.CreateScope()) {
                ReCVEServerContext _context = scope.ServiceProvider.GetService<ReCVEServerContext>();
               if( _context.Clients.Count(c => c.ID == id) == 0) {
                    return true;
               }
                else {
                    return false;
                }
            }
                
        }
    }
    
}
