using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ReCVEServer.Data;
using ReCVEServer.Models;
using ReCVEServer.NistApi;

namespace ReCVEServer.Controllers {
    public class NistController : Controller {
        private readonly NistApiClient _nistApiClient;
        private readonly ReCVEServerContext _context;
        public NistController(NistApiConfig config, ReCVEServerContext context) {
            // Load the API key from the configuration
            _nistApiClient = new NistApiClient(config);
            _context = context;
        }

        public async Task<ActionResult> Index() {
           return View();
        }
        [HttpPost]
        public async Task<ActionResult> QueryAPI() {
            try {

            }
            catch (Exception ex) {
                ViewBag.Error = ex.Message;

            }
            return Ok();
        }
        public async Task<IActionResult> CVEView() {

            return View(await _context.CVEs.ToListAsync());
        }

        // Add this using statement at the top of your controller file


        [HttpGet]
        public async Task<IActionResult> getSeverityRating() {
            var cves = await _context.CVEs.ToListAsync();

            var baseScoreCounts = cves
                .GroupBy(c => c.baseSeverity)
                .Select(g => new {
                    BaseSeverity = g.Key,
                    Count = g.Count()
                })
                .ToList();

            return Json(baseScoreCounts);
        }

    }
}
