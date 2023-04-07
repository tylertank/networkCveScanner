using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ReCVEServer.Networking
{
    public class Server
    {
        public static void main(string[] args)
        {
            serverSock();
        }
       public static async void serverSock()
        {
            System.Diagnostics.Debug.WriteLine("I made it into serverSock");
            IPHostEntry entry = await Dns.GetHostEntryAsync("localhost");
            IPAddress iPAddress = entry.AddressList[0];
            IPEndPoint epEndPoint = new(iPAddress,11_000);

            TcpListener listener = new(epEndPoint);
            Console.WriteLine("We're now waiting for a connection");
            listener.Start();
            using TcpClient handler = await listener.AcceptTcpClientAsync();
            System.Diagnostics.Debug.WriteLine("We got a connection");
            await using NetworkStream stream = handler.GetStream();

            string message = "hello world";
            var enMessage = Encoding.UTF8.GetBytes(message);
            await stream.WriteAsync(enMessage);
            byte[] buffer = new byte[1024];
            await stream.ReadAsync(buffer);
            System.Diagnostics.Debug.WriteLine(buffer.ToString());

        }
    }
}
