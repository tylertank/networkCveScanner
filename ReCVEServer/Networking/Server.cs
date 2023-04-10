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
        public Server(IServiceScopeFactory serviceScopeFactory)//ReCVEServerContext context,
        {
            _scopeFactory = serviceScopeFactory;
            serverSock();
            // _context = context;
        }
        public async Task StartAsync()
        {

            Console.WriteLine("\n\n\nI made it to start async\n\n\n");
        }
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
        //This method sends the json to the appropiate method to be parsed and later stored in the database
        public async void directClient(object obj)
        {
            TcpClient handler = (TcpClient)obj;
            NetworkStream stream = handler.GetStream();
            Task<string> jString = ReceiveData(stream);
            string jsonS = jString.Result;
            var jResults = JObject.Parse(jsonS);
            var IDVal =jResults.GetValue("id");
            var typeVal = jResults.GetValue("type");
            //System.Diagnostics.Debug.WriteLine(print);
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
        public async void clientHandshake(JObject jResults)
        {
            System.Diagnostics.Debug.WriteLine("made it to client handshake");
   
            var info = jResults.GetValue("info");
            var temp = info.First();
            var computer = temp.Value<string>("computer");
            var ip = temp.Value<string>("ip");
            
            //process client hello and get computer addresss and ip out of it
            //create serverAck with client id
            //send server ack to cilent
        }
        public async void processScan(JObject jResults)
        {
            System.Diagnostics.Debug.WriteLine("made it to process scan");
            int id = jResults.Value<int>("id");
            var objects = jResults.GetValue("objects");
            for( int i=0; i < objects.Count(); i++)
            {
                //do something with each object
                System.Diagnostics.Debug.WriteLine(i);
            }
            //how do we store vendor application version?
            //maybe just an array?
            
            //process scan and put in database
        }
        public async void processStatus(JObject jResults)
        {
            System.Diagnostics.Debug.WriteLine("made it to process status");
            int id = jResults.Value<int>("id");
            var objects = jResults.GetValue("objects");
            for (int i = 0; i < objects.Count(); i++)
            {
                //do something with each object
                System.Diagnostics.Debug.WriteLine(i);
            }
            //process status and put in data base
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

