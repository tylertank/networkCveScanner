using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ReCVEServer.Areas.Identity.Data;
using ReCVEServer.Models;
using System.Diagnostics;

namespace ReCVEServer.Data;

public class ReCVEServerContext : IdentityDbContext<ReCVEServerUser>
{
    public ReCVEServerContext(DbContextOptions<ReCVEServerContext> options)
        : base(options)
    {
    }
    public DbSet<Client> Clients { get; set; }
    public DbSet<CVE> CVEs { get; set; }
    public DbSet<Software> Softwares { get; set; }
    public DbSet<Vulnerability> Vulnerabilities { get; set; }
    public DbSet<Status> Statuses { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
    }

    public static class DbInitializer
    {
        public static void InitializeClients(ReCVEServerContext context) {
            context.Database.EnsureCreated();

            List<Client> clients = new List<Client>();

            Client client1 = new Client {
                Name = "Client A",
                IPAddress = "192.168.0.1",
                OS = "Windows",
                OSVersion = "10",
                EnrollmentDate = new DateTime(2022, 3, 15)
            };

            Client client2 = new Client {
                Name = "Client B",
                IPAddress = "192.168.0.2",
                OS = "Linux",
                OSVersion = "Ubuntu 20.04",
                EnrollmentDate = new DateTime(2022, 4, 1)
            };

            Client client3 = new Client {
                Name = "Client C",
                IPAddress = "192.168.0.3",
                OS = "Windows",
                OSVersion = "8.1",
                EnrollmentDate = new DateTime(2022, 2, 10)
            };

            Client client4 = new Client {
                Name = "Client D",
                IPAddress = "192.168.0.4",
                OS = "MacOS",
                OSVersion = "Catalina",
                EnrollmentDate = new DateTime(2022, 1, 5)
            };

            Client client5 = new Client {
                Name = "Client E",
                IPAddress = "192.168.0.5",
                OS = "Windows",
                OSVersion = "11",
                EnrollmentDate = new DateTime(2022, 5, 20)
            };
            context.Clients.Add(client1);
            context.Clients.Add(client2);
            context.Clients.Add(client3);
            context.Clients.Add(client4);
            context.Clients.Add(client5);
             context.SaveChangesAsync();
        }
        public static void InitializeSoftware(ReCVEServerContext context)
        {
            context.Database.EnsureCreated();
            List<Client> clients = context.Clients.ToList();
            Software software1 = new Software {
                client = clients[0],
                vulnerable = "Yes",
                vendor = "Microsoft",
                application = "SQL Server",
                version = "2017"
            };

            Software software2 = new Software {
                client = clients[1],
                vulnerable = "No",
                vendor = "Oracle",
                application = "Java",
                version = "11"
            };

            Software software3 = new Software {
                client = clients[2],
                vulnerable = "Yes",
                vendor = "Google",
                application = "Chrome",
                version = "92"
            };

            Software software4 = new Software {
                client = clients[3],
                vulnerable = "No",
                vendor = "Google",
                application = "Chrome",
                version = "91"
            };

            Software software5 = new Software {
                client = clients[4],
                vulnerable = "Yes",
                vendor = "Adobe",
                application = "Acrobat Reader",
                version = "DC"
            };

            Software software6 = new Software {
                client = clients[0],
                vulnerable = "No",
                vendor = "Microsoft",
                application = "Word",
                version = "365"
            };

            Software software7 = new Software {
                client = clients[1],
                vulnerable = "Yes",
                vendor = "Mozilla",
                application = "Firefox",
                version = "92"
            };

            Software software8 = new Software {
                client = clients[2],
                vulnerable = "No",
                vendor = "Microsoft",
                application = "Excel",
                version = "365"
            };

            Software software9 = new Software {
                client = clients[3],
                vulnerable = "Yes",
                vendor = "Adobe",
                application = "Photoshop",
                version = "2022"
            };

            Software software10 = new Software {
                client = clients[4],
                vulnerable = "No",
                vendor = "Microsoft",
                application = "PowerPoint",
                version = "365"
            };
            context.Softwares.Add(software1);
            context.Softwares.Add(software2);
            context.Softwares.Add(software3);
            context.Softwares.Add(software4);
            context.Softwares.Add(software5);
            context.Softwares.Add(software6);
            context.Softwares.Add(software7);
            context.Softwares.Add(software8);
            context.Softwares.Add(software9);
            context.Softwares.Add(software10);

            context.SaveChanges();
        }
    }
}
