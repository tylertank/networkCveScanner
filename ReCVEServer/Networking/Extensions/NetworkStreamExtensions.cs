///This extension was created by Trace Engel for the ReCVE windows daemon

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;

namespace ReCVEServer.Networking.Extensions
{
    internal static class NetworkStreamExtensions
    {
        public static async Task ReadExactlyAsync(this NetworkStream stream, byte[] buffer, int offset, int count)
        {
            while (count > 0)
            {
                int bytesRead = await stream.ReadAsync(buffer, offset, count); // buffer.AsMemory?
                offset += bytesRead;
                count -= bytesRead;
            }
        }
    }
}
