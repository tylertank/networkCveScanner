using Microsoft.EntityFrameworkCore;
using ReCVEServer.Data;
using ReCVEServer.NistApi;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;

using ReCVEServer.Areas.Identity.Data;
using static ReCVEServer.Data.ReCVEServerContext;
using ReCVEServer.Networking;
using Microsoft.Data.SqlClient;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.AspNetCore.DataProtection;
using ReCVEServer.Controllers;
using ReCVEServer.Networking.ClientProcessing;
using Microsoft.AspNetCore.Http.Features;

// Program fails if SQL container is not yet running. Currently, this program is set to restart on failure.
// Potential solution: ping SQL ports until response.

var builder = WebApplication.CreateBuilder();
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ReCVEServerContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDefaultIdentity<ReCVEServerUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ReCVEServerContext>();
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();


//builder.Services.AddDatabaseDeveloperPageExceptionFilter();
// Add services to the container.
builder.Services.Configure<FormOptions>(options => {
    options.MultipartBodyLengthLimit = 500_000_000;
});
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<NistApiConfig>(provider =>
{
    var configuration = provider.GetService<IConfiguration>();
    return new NistApiConfig(configuration);
});

builder.Services.AddTransient<NistApiClient>();
builder.Services.AddSingleton<ServerLoop>();
builder.Services.AddSingleton<AssemblySender>();
builder.Services.AddHostedService<QueuedHostedService>();
builder.Services.AddSingleton<IBackgroundTaskQueue>(_ => {
    if (!int.TryParse(builder.Configuration["QueueCapacity"], out var queueCapacity)) {
        queueCapacity = 10;
    }
    return new ServerBackgroundTaskQueue(queueCapacity);
});
builder.Services.AddHostedService<DeviceHistoryManager>();

var app = builder.Build();
app.Services.GetService<IHostApplicationLifetime>().ApplicationStarted.Register(() =>
{
    var server = app.Services.GetService<Server>();
    server.StartAsync();
});

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context =
       services.GetRequiredService<ReCVEServerContext>();


    if (context.Database.EnsureCreated())
    {

       await DbInitializer.InitializeClients(context);
    //   await DbInitializer.InitializeSoftware(context);
       await DbInitializer.InitializeStatus(context);
       await DbInitializer.InitializeHistory(context);


    }
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();;

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Nist}/{action=Index}/{id?}");
app.UseEndpoints(endpoints => {
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Device}/{action=Index}/{id?}");
});
app.MapControllers();
app.MapRazorPages();
app.Run();

