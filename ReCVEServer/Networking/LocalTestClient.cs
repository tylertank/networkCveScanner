using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace ReCVEServer.Networking
{
    public class LocalTestClient
    {
        public static async void clientSock()
        {
            Console.WriteLine("I made it into clientSock");
            IPHostEntry entry = await Dns.GetHostEntryAsync("localhost");
            IPAddress iPAddress = entry.AddressList[0];
            IPEndPoint epEndPoint = new(iPAddress, 11_000);

            using TcpClient client = new();
            await client.ConnectAsync(epEndPoint);
            await using NetworkStream stream = client.GetStream();

            var buffer = new byte[1024];
            int recieved = await stream.ReadAsync(buffer);
            Console.WriteLine(buffer.ToString());
            await stream.WriteAsync(buffer);
            Console.WriteLine("sent the string back lol");
        }
    }
}
