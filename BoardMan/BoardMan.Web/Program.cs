using Microsoft.EntityFrameworkCore;
using BoardMan.Web.Data;
using BoardMan.Web.Managers;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

var connectionString = configuration.GetConnectionString("BoardManDbContextConnection"); 
services.AddDbContext<BoardManDbContext>(options => options.UseSqlServer(connectionString)); 
services.AddDefaultIdentity<AppUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<BoardManDbContext>();
services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
services.AddScoped<ISubscriptionManager, SubscriptionManager>();
services.AddScoped<IPlanManager, PlanManager>();

// Add services to the container.
services.AddControllersWithViews();

services.AddAuthentication().AddGoogle(googleOptions =>
{
	googleOptions.ClientId = configuration["Authentication:Google:ClientId"];
	googleOptions.ClientSecret = configuration["Authentication:Google:ClientSecret"];
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
