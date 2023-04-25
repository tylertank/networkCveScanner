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
                await saveCVEData();
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
            var cves = await _context.CVEs.ToListAsync();

            var cveListWithoutDescription = cves.Select(cve => new
            {
                cve.ID,
                cve.cveID,
                cve.published,
                cve.baseScore,
                cve.baseSeverity,
                cve.vendor,
                cve.application,
                cve.version
            }).OrderBy(c => c.baseScore);

            return View(cveListWithoutDescription);
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

        [HttpGet]
        public async Task<IActionResult> getCveHistory() {
            var cves = await _context.History.OrderBy( c => c.date).ToListAsync();
            return Json(cves);

        }

        /// <summary>
        /// This will be called once a day but currently is called when the api is queried, 
        /// It saves the current average CVE score to the database as well as the date for graphing.
        /// </summary>
        /// <returns> nothing</returns>
        public async Task saveCVEData() {
            var cves = await _context.CVEs.ToListAsync();
            var baseScore = cves.Select(c => c.baseScore).ToList();
            
            var baseSeverityCounts = cves
                .GroupBy(c => c.baseSeverity )
                .Select(g => new {
                    BaseSeverity = g.Key,
                    Count = g.Count()
                })
                .ToList();


            CveHistory cveHistory = new CveHistory();
            cveHistory.highCount = 0;
            cveHistory.criticalCount = 0;
            cveHistory.mediumCount = 0;
            cveHistory.lowCount = 0;
            var totalCount = 0;

            foreach (var severity in baseSeverityCounts) {
                if (severity.BaseSeverity == "CRITICAL") {
                    totalCount += severity.Count;
                    cveHistory.criticalCount = severity.Count;
                }else if(severity.BaseSeverity == "HIGH") {
                    totalCount += severity.Count;
                    cveHistory.highCount = severity.Count;
                }else if( severity.BaseSeverity == "MEDIUM") {
                    cveHistory.mediumCount = severity.Count;
                    totalCount += severity.Count;
                }
                else {
                    cveHistory.lowCount = severity.Count;
                    totalCount += severity.Count;
                }
            }

            cveHistory.totalCount = totalCount;
            cveHistory.date = DateTime.Now;
            double averageScore = 0;
            totalCount = 0;
            foreach(var score in baseScore){
                totalCount++;
                averageScore += score;
            }
            averageScore = averageScore / totalCount;
            if( totalCount != 0) {

            cveHistory.cveScore = averageScore;
            }
            else {

            cveHistory.cveScore = 0.0;
            }
            _context.History.Add(cveHistory);
            await _context.SaveChangesAsync();
        }

    }
}
