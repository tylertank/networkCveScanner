using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;
namespace ReCVEServer.Models
{
    public class CVE
    {
        public int ID { get; set; }

        public string cveID { get; set; }
        public string? description { get; set; }
        public DateTime published { get; set; }
        public double baseScore { get; set; }
        public string? baseSeverity {  get; set; }

        public string? vendor { get; set; }

        public string? application { get; set; }

        public string? version { get; set; }
        public ICollection<Vulnerability> Vulnerabilities { get; set; }
    }
}
