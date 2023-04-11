using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ReCVEServer.Areas.Identity.Data;
using ReCVEServer.Models;
using System.Diagnostics;
using System.Reflection.Emit;

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
        builder.Entity<CVE>()
          .HasIndex(c => new { c.cveID, c.vendor, c.application, c.version })
          .IsUnique();


    }

    public static class DbInitializer {
        public static async Task InitializeClients(ReCVEServerContext context) {
            context.Database.EnsureCreated();

            List<Client> clients = new List<Client>();

            Client client1 = new Client {
                Name = "Client A",
                IPAddress = "192.168.0.1",
                OS = "Windows 10",
                OSVersion = "20h2",
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
                OS = "Windows 10",
                OSVersion = "1903",
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
                OS = "Windows 11",
                OSVersion = "22h2",
                EnrollmentDate = new DateTime(2022, 5, 20)
            };
            context.Clients.Add(client1);
            context.Clients.Add(client2);
            context.Clients.Add(client3);
            context.Clients.Add(client4);
            context.Clients.Add(client5);
            await context.SaveChangesAsync();
        }
        public static async Task InitializeSoftware(ReCVEServerContext context) {
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

            await context.SaveChangesAsync();
        }
        public static async Task InitializeStatus(ReCVEServerContext context) {
            Status status1 = new Status {
                clientID = 1,
                processStatus = "Running",
                cpu = 1.25f,
                memory = 0.00f
            };

            Status status2 = new Status {
                clientID = 1,
                processStatus = "Stopped",
                cpu = 0.0f,
                memory = 0.0f
            };

            Status status3 = new Status {
                clientID = 1,
                processStatus = "Error",
                cpu = 0.0f,
                memory = 0.0f
            };

            Status status4 = new Status {
                clientID = 1,
                processStatus = "Idle",
                cpu = 0.00f,
                memory = 1.10f
            };

            Status status5 = new Status {
                clientID = 1,
                processStatus = "Busy",
                cpu = 10.75f,
                memory = 10.00f
            };
            context.Statuses.Add(status1);
            context.Statuses.Add(status2);
            context.Statuses.Add(status3);
            context.Statuses.Add(status4);
            context.Statuses.Add(status5);
            await context.SaveChangesAsync();

        }
    }
}
