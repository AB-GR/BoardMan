using Microsoft.EntityFrameworkCore;
using BoardMan.Web.Data;
using BoardMan.Web.Managers;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.Extensions.Options;
using Stripe;

var builder = WebApplication.CreateBuilder(args);
var services = builder.Services;
var configuration = builder.Configuration;

var connectionString = configuration.GetConnectionString("BoardManDbContextConnection"); 
services.AddDbContext<BoardManDbContext>(options => options.UseSqlServer(connectionString)); 
services.AddDefaultIdentity<AppUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<BoardManDbContext>();
services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
services.AddScoped<ISubscriptionManager, SubscriptionManager>();
services.AddScoped<IPlanManager, PlanManager>();
services.AddScoped<IPaymentManager, PaymentManager>();
services.AddScoped<IWorkspaceManager, WorkspaceManager>();
services.AddScoped<IBoardManager, BoardManager>();
services.AddScoped<IListManager, ListManager>();
services.AddScoped<ITaskManager, TaskManager>();
services.AddTransient<PaymentIntentService>();
services.AddScoped<IPaymentService, PaymentService>();
services.AddLocalization(o => o.ResourcesPath = "Resources");
services.AddMvc()
 .AddViewLocalization(LanguageViewLocationExpanderFormat.Suffix) 
 .AddDataAnnotationsLocalization();

services.Configure<RequestLocalizationOptions>(options =>
{
    List<CultureInfo> supportedCultures = new List<CultureInfo>
        {
            new CultureInfo("en-US"),
            new CultureInfo("de-DE")
        };

    options.DefaultRequestCulture = new RequestCulture("en-GB");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});

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

var supportedCultures = new[] { "en-US", "de-DE" };

var options = app.Services.GetService<IOptions<RequestLocalizationOptions>>();
if(options != null)
    app.UseRequestLocalization(options.Value);

app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

StripeConfiguration.ApiKey = configuration.GetValue<string>("StripeSecretKey");
app.Run();
