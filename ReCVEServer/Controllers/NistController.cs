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
            _nistApiClient = new NistApiClient(config, context);
            _context = context;
        }
        /// <summary>
        /// Displays the index view.
        /// </summary>
        /// <returns>The index view.</returns>
        public async Task<ActionResult> Index() {
           return View();
        }
        /// <summary>
        /// Makes a request to the NIST API to check the software version.
        /// </summary>
        /// <returns>An HTTP OK response.</returns>
        [HttpPost]
        public async Task<ActionResult> QueryAPI() {
            try {
               await _nistApiClient.checkSoftware();
            }
            catch (Exception ex) {
                ViewBag.Error = ex.Message;

            }
            return Ok();
        }
        /// <summary>
        /// Displays a view that shows all the CVEs in the database.
        /// </summary>
        /// <returns>The CVE view.</returns>
        public async Task<IActionResult> CVEView() {

            return View(await _context.CVEs.ToListAsync());
        }



        /// <summary>
        /// Retrieves the number of CVEs with each base severity rating.
        /// </summary>
        /// <returns>A JSON representation of the base severity counts.</returns>
        [HttpGet]
        public async Task<IActionResult> getSeverityRating() {
            var cves = await _context.CVEs.ToListAsync();
            // Group the CVEs by their base severity rating and count the number of CVEs in each group
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
