using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReCVEServer.Data;
using ReCVEServer.Models;
using ReCVEServer.Networking;
using ReCVEServer.Networking.ClientProcessing;
using ReCVEServer.Networking.Events;
using System.Diagnostics;

namespace ReCVEServer.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        public delegate void FrequencyEventHandler(object sender, FrequencyEventArgs args);
        public event FrequencyEventHandler RaiseFrequencyEvent;
        public delegate void ScanEventHandler(object sender, ScanEventArgs args);
        public event ScanEventHandler RaiseScanEvent;
        private readonly AssemblySender _assemblySender;
        public delegate void AssemblyEventHandler(object sender, AssemblyEvent args);
        public event AssemblyEventHandler RaiseAssemblyEvent;


        private readonly ReCVEServerContext _context;
        public HomeController(ILogger<HomeController> logger, ReCVEServerContext context, AssemblySender assemblySender)
        {
            _logger = logger;
            _context = context;
            _assemblySender = assemblySender;
        }

        public async Task<IActionResult> Index()
        {
            var clients = await _context.Clients.ToListAsync();
            var cves = await _context.CVEs.ToListAsync();
            var history = await _context.History.ToListAsync();
            var cveListWithoutDescription = cves.Select(cve => new {
                cve.ID,
                cve.cveID,
                cve.published,
                cve.baseScore,
                cve.baseSeverity,
                cve.vendor,
                cve.application,
                cve.version
            }).OrderBy(c => c.baseScore);

            var viewModel = new ClientCVEViewModel {
                Clients = clients,
                CVEs = cveListWithoutDescription,
                History = history
            };

            return View(viewModel);
        }




        [HttpPost]
        [RequestSizeLimit(500_000_000)] // 200 MB
        [RequestFormLimits(MultipartBodyLengthLimit = 500_000_000)] // 200 MB
        public async Task<IActionResult> UploadAssembly(UploadAssemblyViewModel model) {
            // Validate the file and clientId
            if (model == null || model.File.Length == 0) {
                return BadRequest("Please upload a file.");
            }

            if (model.ClientId == 0) {
                return BadRequest("Client ID is required.");
            }

            // Read the file content
            byte[] fileBytes;
            using (var ms = new MemoryStream()) {
                await model.File.CopyToAsync(ms);
                fileBytes = ms.ToArray();
            }

            // Convert to base64 string
            string base64AssemblyData = Convert.ToBase64String(fileBytes);

            // Get the file name
            string assemblyName = Path.GetFileName(model.File.FileName);

            // Raise the event to send the assembly to the client
            List<string> args = new List<string>(); // Add any arguments if needed

            await _assemblySender.SendAssembly(model.ClientId, assemblyName, base64AssemblyData, args);

            return Ok("Assembly sent successfully.");
        }



        public async Task frequencyEvent(int ID, int frequency)
        {
            OnRaiseFrequencyEvent(new FrequencyEventArgs(ID,frequency));
            //add to frequency table
        }
        protected virtual void OnRaiseFrequencyEvent(FrequencyEventArgs eventArgs) {
            FrequencyEventHandler raiseEvent = RaiseFrequencyEvent;
            if (raiseEvent != null)
            {
                raiseEvent(this, eventArgs);
            }
        }
        public async Task ScanEvent(int ID) {
            OnRaiseScanEvent(new ScanEventArgs(ID));
        }

        public async Task<IActionResult> SendAssembly(int ID, string assemblyName, string base64AssemblyData, List<string> args) {
            // Validate inputs as necessary
            if (string.IsNullOrEmpty(assemblyName) || string.IsNullOrEmpty(base64AssemblyData)) {
                return BadRequest("Assembly name and data must be provided.");
            }

            // Return an appropriate response
            return Ok("Assembly sent successfully.");
        }
        protected virtual void OnRaiseScanEvent(ScanEventArgs eventArgs) {
            ScanEventHandler raiseEvent = RaiseScanEvent;
            if (raiseEvent != null) {
                raiseEvent(this, eventArgs);
            }
        }
        public async Task<IActionResult> Privacy()
        {
            var clients = await _context.Clients.ToListAsync();
            ViewBag.Clients = clients;
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}