using Microsoft.AspNetCore.Mvc;
using ReCVEServer.NistApi;

namespace ReCVEServer.Controllers
{
    public class NistController : Controller
    {
        private readonly NistApiClient _nistApiClient;

        public NistController(NistApiConfig config)
        {
            // Load the API key from the configuration
            _nistApiClient = new NistApiClient(config);
        }

        public async Task<ActionResult> Index()
        {
            int startIndex = 0;
            int resultsPerPage = 20;

            try
            {
                var cveData = await _nistApiClient.GetCveDataAsync(startIndex, resultsPerPage);
                ViewBag.CveData = cveData;
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
           
            }
                return View();
        }
    }
}
