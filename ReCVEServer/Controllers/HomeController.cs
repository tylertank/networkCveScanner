using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReCVEServer.Data;
using ReCVEServer.Models;
using System.Diagnostics;

namespace ReCVEServer.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;


        private readonly ReCVEServerContext _context;


        public HomeController(ILogger<HomeController> logger, ReCVEServerContext context)
        {
            _logger = logger;
            _context = context;
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

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}