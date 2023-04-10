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

namespace ReCVEServer.Networking
{

    public class Server
    {
        private readonly ReCVEServerContext _context;
        private readonly IServiceScopeFactory _scopeFactory;
        /// <summary>
        ///  This is the server constructor for the server class
        ///  In order for server to be called in the program.cs builder we need to alter the scope of 
        ///  the server class
        ///  we do this using iservicescopefactory as seen below
        /// </summary>


        public Server(IServiceScopeFactory serviceScopeFactory)//ReCVEServerContext context,
        {
            _scopeFactory = serviceScopeFactory;
            serverSock();
            // _context = context;
        }

        /// <summary>
        ///  This also helps the builder in program.cs run the server class
        /// </summary>

        public async Task StartAsync()
        {

            Console.WriteLine("\n\n\nI made it to start async\n\n\n");
        }
        /// <summary>
        /// This is the landing method for server, this is where the server is started 
        /// connecting clients are given seperate threads from here to interact with the server
        /// </summary>

        public async void serverSock()
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var context = scope.ServiceProvider.GetService<ReCVEServerContext>();
                // var contextList = context.Softwares.ToList();

                System.Diagnostics.Debug.WriteLine("I made it into serverSock");
                IPEndPoint epEndPoint = new(IPAddress.Any, 5004);
                TcpListener listener = new(epEndPoint);

                System.Diagnostics.Debug.WriteLine("We're now waiting for a connection");
                listener.Start(10);

                while (true)
                {
                    TcpClient handler = await listener.AcceptTcpClientAsync();

                    Thread t = new Thread(new ParameterizedThreadStart(directClient));
                    t.Start(handler);
                }

            }
        }
        /// <summary>
        ///  when the server recieves a json from the client it'll be directed here to be parsed
        ///  depending on the type of json it'll be sent to the appropiate function to have the 
        ///  information extracted
        /// </summary>

        private void directClient(object obj)
        {
            TcpClient handler = (TcpClient)obj;
            NetworkStream stream = handler.GetStream();
            Task<string> jString = ReceiveData(stream);
            string jsonS = jString.Result;
            var jResults = JObject.Parse(jsonS);
            var IDVal =jResults.GetValue("id");
            var typeVal = jResults.GetValue("type");
            if (jResults.Value<string>("id") == "null")
            {
                clientHandshake(jResults);
            }
            else if (jResults.Value<string>("type") == "scan")
            {
                processScan(jResults);
            }
            else if(jResults.Value<string>("type") == "process")
            {
                processStatus(jResults);
            }
           handler.Close();
        }
        /// <summary>
        ///  When a client connects for the first time it'll send a client handshake json
        ///  this'll give the server the needed information to identify the client in the database
        ///  the server will also send an ack json to the client so they know what their client id is 
        ///  going forward
        /// </summary>

        private void clientHandshake(JObject jResults)
        {
            System.Diagnostics.Debug.WriteLine("made it to client handshake");

            var info = jResults.GetValue("info");
            var temp = info.First();
            var computer = temp.Value<string>("computer");
            var ip = temp.Value<string>("ip");
            //send server ack to cilent
        }
        /// <summary>
        ///  When the client sends a scan of their machine it'll be processed here
        /// </summary>

        private void processScan(JObject jResults)
        {
            System.Diagnostics.Debug.WriteLine("made it to process scan");
            int id = jResults.Value<int>("id");
            var objects = jResults.GetValue("objects");
            for( int i=0; i < objects.Count(); i++)
            {
                //do something with each object
                System.Diagnostics.Debug.WriteLine(i);
            }
        }
        /// <summary>
        ///  When the client sends an updated status think task manager type information
        ///  it'll be processed here
        /// </summary>

        private void processStatus(JObject jResults)
        {
            System.Diagnostics.Debug.WriteLine("made it to process status");
            int id = jResults.Value<int>("id");
            var objects = jResults.GetValue("objects");
            for (int i = 0; i < objects.Count(); i++)
            {
                //do something with each object
                System.Diagnostics.Debug.WriteLine(i);
            }
        }


        //***************************************************************************************************************************
        //These methods are modified fromm the windows daemon created by trace engel
        
        private void SendData(string message, NetworkStream stream)
        {
            byte[] data = new byte[message.Length + 4];

            byte[] count_bytes = BitConverter.GetBytes(message.Length);
            for (int i = 0; i < 4; i++)
            {
                data[i] = count_bytes[i];
            }
            Encoding.UTF8.GetBytes(message, 0, message.Length, data, 4);

            //NetworkStream stream = _tcpSocket.GetStream();
            //try
            {
                stream.Write(data, 0, data.Length);
            }
        }
        private async Task<string> ReceiveData(NetworkStream stream)
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

