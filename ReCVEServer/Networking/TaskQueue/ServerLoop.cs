using ReCVEServer.Data;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Linq;
using static ReCVEServer.Data.ReCVEServerContext;
using ReCVEServer.Models;
using static System.Formats.Asn1.AsnWriter;
using Newtonsoft.Json;
using System.Collections;
using CommonReCVE.Crypto;
using CommonReCVE.Extensions;
using ReCVEServer.Networking.ScheduledScan;
using ReCVEServer.Controllers;
using System.Security.Cryptography.X509Certificates;
using ReCVEServer.Networking.ClientProcessing;

namespace ReCVEServer.Networking 
    { 
    public class ServerLoop
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IBackgroundTaskQueue _taskQueue;
        private readonly ILogger<ServerLoop> _logger;
        private readonly CancellationToken _cancellationToken;
        private readonly HomeController _controller;
        private ScheduleScan scheduleScan;
        private readonly AssemblySender _assemblySender;

        internal static string _base_path = @"..";

        public static string PublicKey { get 
            {
                return Path.Combine(_base_path, "keys", "server_key.der");
            } }

        private static string PrivateKey { get
            {
                return Path.Combine(_base_path, "keys", "server_priv");
            } }


        /// <summary>
        /// ServerLoop is where the tcp server for ReCve is set up, and where it manages intitial connections
        /// with clients. When a client connects serverLoop will make a work task which will be 
        /// put in the taskqueue so all of the client information can be processed in a seperate thread. 
        /// </summary>
        public ServerLoop(
            IServiceScopeFactory serviceScopeFactory,
            IBackgroundTaskQueue taskQueue,
            ILogger<ServerLoop> logger,
            IHostApplicationLifetime applicationLifetime,
            AssemblySender assemblySender)
        {
            _scopeFactory = serviceScopeFactory;
            _taskQueue = taskQueue;
            _logger = logger;
            _cancellationToken = applicationLifetime.ApplicationStopping;
          scheduleScan = new ScheduleScan(_scopeFactory);
            _assemblySender = assemblySender;
        }
        /// <summary>
        /// This logs that we've made it to the start of the server 
        /// </summary>
        public void startSever() {
            _logger.LogInformation($"{nameof(ServerAsync)} loop is starting.");
            
            Task.Run(() => {
                ServerAsync();
            }).ConfigureAwait(false);
            Task.Run(async () => {
                await Task.Delay(TimeSpan.FromSeconds(2));
                await ScheduledScan();
            }).ConfigureAwait(false);
            Task.Run(() => {
                UDPBroadcastResponder();
            }).ConfigureAwait(false);
        }

        /// <summary>
        /// This is the main tcp server loop. It creates a task to be completed everytime it makes a new connection. 
        /// </summary>
        private async Task ServerAsync()
        {
            TryLoadKeys();

            IPEndPoint epEndPoint = new(IPAddress.Any, 5002);
            TcpListener listener = new(epEndPoint);
            listener.Start(10);
            _logger.LogInformation("The Tcp Server is now accepting client connections");
            while (!_cancellationToken.IsCancellationRequested) {
                TcpClient handler = await listener.AcceptTcpClientAsync();
                _logger.LogInformation("New client connection accepted");
               
                await _taskQueue.QueueBackgroundWorkItemAsync(async cancellationToken => {
                    ProcessClient server = new ProcessClient(_scopeFactory, _logger,scheduleScan, _assemblySender);
                    server.directClient(handler);
                });
            }
        }
        /// <summary>
        /// This will run every 30 minutes
        /// </summary>
        private async Task ScheduledScan() {
            while (!_cancellationToken.IsCancellationRequested) {
                await _taskQueue.QueueBackgroundWorkItemAsync(async cancellationToken => {
                    scheduleScan.CompareTime(DateTime.Now);
                });
                await Task.Delay(TimeSpan.FromSeconds(600));
            }
        }
        private void UDPBroadcastResponder() {
            UdpClient udpcli = new(new IPEndPoint(IPAddress.Any, 22422));

            if (System.OperatingSystem.IsWindows()) {
                const uint IOC_IN = 0x80000000U;
                const uint IOC_VENDOR = 0x18000000U;

                /// <summary>
                /// Controls whether UDP PORT_UNREACHABLE messages are reported. 
                /// </summary>
                const int SIO_UDP_CONNRESET = unchecked((int)(IOC_IN | IOC_VENDOR | 12));
                udpcli.Client.IOControl(SIO_UDP_CONNRESET, new byte[] { 0x00 }, null);
            }

            while (!_cancellationToken.IsCancellationRequested) {
                try {
                    IPEndPoint from = new(0, 0);
                    string msg = Encoding.UTF8.GetString(udpcli.Receive(ref from));
                    _logger.LogInformation($"Received UDP broadcast: {msg}\nFrom: {from}");
                    if (msg != "RECVE_SERVER_IP_REQUEST") continue;

                    byte[] response = Encoding.UTF8.GetBytes("RECVE_SERVER_IP_RESPONSE");
                    udpcli.Send(response, response.Length, from);
                    _logger.LogInformation($"Sent UDP response to {from}");
                }
                catch (Exception ex) {
                    _logger.LogWarning($"Error in UDPBroadcastResponder(): {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Try to load keys into the given cryptotunnel. If they don't exist, tunnel's pre-generated keys will be saved.
        /// </summary>
        /// <param name="tunnel"></param>
        internal static void TryLoadKeys(CryptoTunnel tunnel) {
            
            // The password needs to be changed, can't be hardcoded. Maybe link it to a login password.
            try {
                // Public key is regenerated from private key
                tunnel.LoadPrivateFromFile(PrivateKey, "password");
            }
            catch (Exception) {
                tunnel.SavePrivateToFile(PrivateKey, "password");
                tunnel.SavePublicToFile(PublicKey);
            }
        }

        private static void TryLoadKeys() {
            // Attempt to generate and save new keys
            CryptoTunnel temp = new();
            TryLoadKeys(temp);
        }
    }
}

