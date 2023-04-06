using Microsoft.AspNetCore.Mvc;
using ReCVEServer.Data;
using ReCVEServer.Models;
using ReCVEServer.NistApi;

namespace ReCVEServer.Controllers
{
    public class NistController : Controller
    {
        private readonly NistApiClient _nistApiClient;
        private readonly ReCVEServerContext _context;
        public NistController(NistApiConfig config, ReCVEServerContext context)
        {
            // Load the API key from the configuration
            _nistApiClient = new NistApiClient(config);
            _context = context;
        }

        public async Task<ActionResult> Index()
        {
            int startIndex = 0;
            string application = "windows_10";
            string version = "22h2";
            string vendor = "microsoft";
            int count = 0;
            try
            {
                CVE tempCVE;
                List<CVE> CVEs = new List<CVE>();
              
                var allVulnerabilities = await _nistApiClient.FetchAllVulnerabilitiesAsync(application, vendor, version);

                foreach( var vulnerability in allVulnerabilities )
                {
                   count++;
                    tempCVE = new CVE();
                    tempCVE.cveID = vulnerability.Data.Id;
                    tempCVE.published = vulnerability.Data.Published;
                    if(vulnerability.Data.Metrics.CvssMetricV3List != null)
                    {

                    tempCVE.baseScore = vulnerability.Data.Metrics.CvssMetricV3List.First().CvssData.BaseScore;
                    }
                    else
                    {
                        tempCVE.baseScore = -1.0;
                    }
                    tempCVE.description = vulnerability.Data.Descriptions.First().Value;
                    _context.CVEs.Add(tempCVE);
                        }
                await _context.SaveChangesAsync();

                ViewBag.CveData = _context.CVEs.ToList();
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
           
            }
                return View();
        }
    }
}
