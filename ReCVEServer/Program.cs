using Microsoft.EntityFrameworkCore;
using ReCVEServer.Data;
using ReCVEServer.NistApi;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using ReCVEServer.Areas.Identity.Data;
using static ReCVEServer.Data.ReCVEServerContext;
using ReCVEServer.Networking;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ReCVEServerContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDefaultIdentity<ReCVEServerUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddEntityFrameworkStores<ReCVEServerContext>();
builder.Services.AddDatabaseDeveloperPageExceptionFilter();


//builder.Services.AddDatabaseDeveloperPageExceptionFilter();
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddSingleton<NistApiConfig>(provider =>
{
    var configuration = provider.GetService<IConfiguration>();
    return new NistApiConfig(configuration);
});

builder.Services.AddTransient<NistApiClient>();
builder.Services.AddSingleton<Server>();

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
    context.Database.EnsureCreated();
    DbInitializer.Initialize(context);
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
app.MapControllers();
app.MapRazorPages();
app.Run();

