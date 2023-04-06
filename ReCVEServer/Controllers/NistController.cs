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
            int startIndex = 0;
            string application = "windows_10";
            string version = "22h2";
            string vendor = "microsoft";
            int count = 0;
            try {
                CVE tempCVE;
                List<CVE> CVEs = new List<CVE>();

                var allVulnerabilities = await _nistApiClient.FetchAllVulnerabilitiesAsync(application, vendor, version);

                foreach (var vulnerability in allVulnerabilities) {
                    count++;
                    tempCVE = new CVE();
                    tempCVE.cveID = vulnerability.Data.Id;
                    tempCVE.published = vulnerability.Data.Published;

                    if (vulnerability.Data.Metrics.CvssMetricV3List != null) {
                        tempCVE.baseSeverity = vulnerability.Data.Metrics.CvssMetricV3List.First().CvssData.Severity;
                        tempCVE.baseScore = vulnerability.Data.Metrics.CvssMetricV3List.First().CvssData.BaseScore;
                    }
                    else if (vulnerability.Data.Metrics.CvssMetricV31List != null) {
                        tempCVE.baseSeverity = vulnerability.Data.Metrics.CvssMetricV31List.First().CvssData.Severity;
                        tempCVE.baseScore = vulnerability.Data.Metrics.CvssMetricV31List.First().CvssData.BaseScore;
                    }
                    else if (vulnerability.Data.Metrics.CvssMetricV2List != null) {
                        tempCVE.baseSeverity = vulnerability.Data.Metrics.CvssMetricV2List.First().Severity;
                        tempCVE.baseScore = vulnerability.Data.Metrics.CvssMetricV2List.First().CvssData.BaseScore;
                    }
                    else {

                        tempCVE.baseScore = -1.0;
                        tempCVE.baseSeverity = "notFound";
                    }
                    tempCVE.description = vulnerability.Data.Descriptions.First().Value;
                    _context.CVEs.Add(tempCVE);
                }
                await _context.SaveChangesAsync();
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
