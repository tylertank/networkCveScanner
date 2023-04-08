using ReCVEServer.Data;
using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ReCVEServer.Networking
{

    public class Server
    {
        private readonly ReCVEServerContext _context;
        private readonly IServiceScopeFactory _scopeFactory;
        public Server( IServiceScopeFactory serviceScopeFactory)//ReCVEServerContext context,
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
                var contextList = context.Softwares.ToList();

                System.Diagnostics.Debug.WriteLine("I made it into serverSock");
                IPHostEntry entry = await Dns.GetHostEntryAsync("localhost");
                IPAddress iPAddress = entry.AddressList[0];
                System.Diagnostics.Debug.WriteLine(entry.AddressList[0]);
                IPEndPoint epEndPoint = new(IPAddress.Any, 5004);

                TcpListener listener = new(epEndPoint);
                System.Diagnostics.Debug.WriteLine("We're now waiting for a connection");
                listener.Start(10);
                while (true)
                {
                    using TcpClient handler = await listener.AcceptTcpClientAsync();
                    System.Diagnostics.Debug.WriteLine("We got a connection");
                    await using NetworkStream stream = handler.GetStream();

                    string message = "hello world";
                    var enMessage = Encoding.UTF8.GetBytes(message);
                    byte[] buffer2 = new byte[1024];
                    await stream.ReadAsync(buffer2);
                    System.Diagnostics.Debug.WriteLine("Recieving first message from client " + Encoding.UTF8.GetString(buffer2) + "\n\n");


                    System.Diagnostics.Debug.WriteLine("Sending first message to client " + message + "\n");
                    await stream.WriteAsync(enMessage);

                    byte[] buffer = new byte[102];
                    await stream.ReadAsync(buffer);
                    Decoder d = Encoding.UTF8.GetDecoder();
                    System.Diagnostics.Debug.WriteLine("recieving second message from client " + Encoding.UTF8.GetString(buffer) + "\n\n");
                    handler.Close();
                }

            }
        }
    }
}
