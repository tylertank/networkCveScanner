    using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NuGet.Protocol;
using ReCVEServer.Data;
using ReCVEServer.Models;
using ReCVEServer.NistApi;
using System.Threading.Tasks;
namespace ReCVEServer.Controllers {

    public class DeviceController : Controller {
        private readonly ReCVEServerContext _context;

        public DeviceController(ReCVEServerContext context) {
            _context = context;
        }
        public async Task<ActionResult> Index() {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> UpdateSystemInfo([FromBody] Status systemInfo) {
            var existingSystemInfo = await _context.Statuses.FindAsync(1);
  
            ReCVEServer.Models.Status tempStatus = new Status();
    
            tempStatus.memory = systemInfo.memory;
            tempStatus.cpu = systemInfo.cpu;
            tempStatus.processStatus = systemInfo.processStatus;


            if (existingSystemInfo == null) {
                tempStatus.processStatus = DateTime.UtcNow.ToString();
                _context.Statuses.Add(tempStatus);
            }
            else {
                existingSystemInfo.cpu = tempStatus.cpu;
                existingSystemInfo.memory= tempStatus.memory;
                existingSystemInfo.processStatus= tempStatus.processStatus.ToString();
            }

            await _context.SaveChangesAsync();
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetSystemInfo() {
            var systemInfo = await _context.Statuses.FindAsync(1);

            if (systemInfo == null) {
                return NotFound();
            }
       
            return Json(systemInfo);
        }

   
    }
}
