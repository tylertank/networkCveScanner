using ReCVEServer.Data;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using ReCVEServer.Networking.Extensions;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Linq;
using static ReCVEServer.Data.ReCVEServerContext;
using ReCVEServer.Models;
using static System.Formats.Asn1.AsnWriter;
using Newtonsoft.Json;
//using ReCVEServer.Networking.ServerAck;

namespace ReCVEServer.Networking
{

    public class Server
    {
        private readonly IServiceScopeFactory _scopeFactory;

        /// <summary>
        ///  This is the server constructor for the server class
        ///  In order for server to be called in the program.cs builder we need to alter the scope of 
        ///  the server class
        ///  we do this using iservicescopefactory as seen below
        /// </summary>
        public Server(IServiceScopeFactory serviceScopeFactory)
        {
            _scopeFactory = serviceScopeFactory;
             serverSock();
        }

        /// <summary>
        ///  This also helps the builder in program.cs run the server class
        /// </summary>
        public async Task StartAsync()
        {
        }

        /// <summary>
        /// This is the landing method for server, this is where the server is started 
        /// connecting clients are given seperate threads from here to interact with the server
        /// </summary>
        public async void serverSock()
        {
                System.Diagnostics.Debug.WriteLine("I made it into serverSock");
                IPEndPoint epEndPoint = new(IPAddress.Any, 5004);
                TcpListener listener = new(epEndPoint);

                System.Diagnostics.Debug.WriteLine("We're now waiting for a connection");
                listener.Start(10);

                while (true)
                {
                
                try {
                    TcpClient handler = await listener.AcceptTcpClientAsync();
                    Thread t = new Thread(new ParameterizedThreadStart(directClient));
                    t.Start(handler);
                }
                catch (Exception E) {
                        
                    }
                    
                }
        }

        /// <summary>
        ///  when the server recieves a json from the client it'll be directed here to be parsed
        ///  depending on the type of json it'll be sent to the appropiate function to have the 
        ///  information extracted
        /// </summary>
        /// fix client hello, it should be sent before anything else
        private async void directClient(object obj)
        {
            try {

                TcpClient handler = (TcpClient)obj;
                NetworkStream stream = handler.GetStream();

                while (handler.Connected) {
                    Task<string> jString = ReceiveData(stream);
                    jString.Wait();
                    string jsonS = jString.Result;
                    var jResults = JObject.Parse(jsonS);

                    if (jResults.Value<string>("type") == "clientHello") {
                        Task<string> json = startHandshake(jResults);
                        json.Wait();
                        SendData(json.Result, stream);
                        ServerCommand command = new ServerCommand();
                        command.command = "scan";
                        string json2 = JsonConvert.SerializeObject(command);
                        SendData(json2, stream);
                    }
                    else if (jResults.Value<string>("type") == "scan") {
                        await processScan(jResults);
                        
                    }
                    else if (jResults.Value<string>("type") == "process") {
                        await processStatus(jResults);
                       
                    }
                }
                handler.Close();
            }
            catch (Exception ex) {
                Console.WriteLine("something was closed", ex.ToString());
            }
        }
        /// <summary>
        /// This code checks if the client is a returning client or a first time client. 
        /// </summary>
        /// <param name="jResults"></param>
        /// <returns></returns>
        private async Task<string> startHandshake(JObject jResults) {
            ServerAck serverAck = new ServerAck();
            //If this is the first time a client is connecting go here
            if (jResults.Value<int>("id") == 0) {
                Task<int> clientID = clientHandshake(jResults);
                clientID.Wait();
                serverAck.id = clientID.Result;
            }
            //If the client is just reconnecting go here
            else {
                serverAck.id = jResults.Value<int>("id");
            }
            return JsonConvert.SerializeObject(serverAck);
        }
        /// <summary>
        ///  When a client connects for the first time it'll send a client handshake json
        ///  this'll give the server the needed information to identify the client in the database
        ///  the server will also send an ack json to the client so they know what their client id is 
        ///  going forward
        /// </summary>

        private async Task<int> clientHandshake(JObject jResults)
        {
            System.Diagnostics.Debug.WriteLine("made it to client handshake");
            using (var scope = _scopeFactory.CreateScope())
            {
                ReCVEServerContext _context = scope.ServiceProvider.GetService<ReCVEServerContext>();
                //Extract the data from the json
                var temp = jResults.GetValue("info");
                var computer = temp.Value<string>("computer");
                var ip = temp.Value<string>("ip");
                var os = "Windows_10";//temp.Value<string>("OS");
                var osVersion = "22H2";// = temp.Value<string>("OSVersion");

                //parse the data into a client object
                Client client = new Client();
                client.IPAddress = ip;
                client.Name = computer;
                client.OS = os;
                client.OSVersion = osVersion;
                client.EnrollmentDate = DateTime.Now;

                //add the client object to the database  and save it
                _context.Clients.Add(client);
                await _context.SaveChangesAsync();
                return client.ID;
            }
        }

        /// <summary>
        ///  When the client sends a scan of their machine it'll be processed here
        /// </summary>

        private async Task processScan(JObject jResults)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                ReCVEServerContext _context = scope.ServiceProvider.GetService<ReCVEServerContext>();
                System.Diagnostics.Debug.WriteLine("made it to process scan");
                int id = jResults.Value<int>("id");
                var objects = jResults.GetValue("objects");
                var cliList = _context.Clients.ToList();
                for (int i = 0; i < objects.Count(); i++)
                {
                    Software tempSoftware = new Software();
                    var current = objects[i];
                    tempSoftware.vendor = current.Value<string>("vendor");
                    tempSoftware.application = current.Value<string>("application");
                    tempSoftware.version = current.Value<string>("version");
                    tempSoftware.client = cliList[id - 1];
                    _context.Softwares.Add(tempSoftware);
                }
                await _context.SaveChangesAsync();
            }
        }
        /// <summary>
        ///  When the client sends an updated status think task manager type information
        ///  it'll be processed here
        /// </summary>

        private async Task processStatus(JObject jResults)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                ReCVEServerContext _context = scope.ServiceProvider.GetService<ReCVEServerContext>();
                System.Diagnostics.Debug.WriteLine("made it to process status");
                int id = jResults.Value<int>("id");
                var objects = jResults.GetValue("objects");
                for (int i = 0; i < objects.Count(); i++)
                {
                    var current = objects[i];
                    Status tempStatus = new Status();
                    tempStatus.clientID = id;
                    tempStatus.memory = current.Value<float>("mem");
                    tempStatus.processStatus = current.Value<string>("process");
                    tempStatus.cpu = current.Value<float>("cpu");
                    var existingStatus = _context.Statuses.FirstOrDefault(s => s.clientID == tempStatus.clientID && s.processStatus == tempStatus.processStatus);

                    if (existingStatus != null) {
                        // Update the existing entry
                        existingStatus.memory = tempStatus.memory;
                        existingStatus.cpu = tempStatus.cpu;
                    }
                    else {
                        // Add a new entry
                        _context.Statuses.Add(tempStatus);
                    }
                }
                await _context.SaveChangesAsync();
            }
        }

        //***************************************************************************************************************************
        //These methods are modified fromm the windows daemon created by trace engel
        
        public void SendData(string message, NetworkStream stream)
        {
            byte[] data = Encoding.UTF8.GetBytes(message, 0, message.Length);
            byte[] count_bytes = BitConverter.GetBytes(data.Length);
           stream.Write(count_bytes, 0, 4);
           stream.Write(data, 0, data.Length);
        }
        public async Task<string> ReceiveData(NetworkStream stream)
        {
            byte[] buffer = new byte[1024]; // Note the buffer is never cleared. It is simply overwritten with new data
                                            // and only the new data is read.
            await stream.ReadExactlyAsync(buffer, 0, 4);
            int dataLength = BitConverter.ToInt32(buffer, 0);
            StringBuilder sb = new();
            while (dataLength > 0)
            {
                int bytesRead = await stream.ReadAsync(buffer, 0, dataLength < 1024 ? dataLength : 1024);
                sb.Append(Encoding.UTF8.GetString(buffer, 0, bytesRead));
                dataLength -= bytesRead;
            }
            return sb.ToString();
        }
    }
}

