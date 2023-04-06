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
        public static void Initialize(ReCVEServerContext context)
        {
            context.Database.EnsureCreated();

            
            context.SaveChanges();
        }
    }
}
