using BoardMan.Web.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace BoardMan.Web.Controllers
{
	public abstract class SiteControllerBase : Controller
	{		
		protected readonly UserManager<AppUser> userManager;
		protected readonly IConfiguration configuration;
		protected readonly ILogger logger;

		protected SiteControllerBase(UserManager<AppUser> userManager, IConfiguration configuration, ILogger logger)
		{
			this.userManager = userManager;
			this.configuration = configuration;
			this.logger = logger;
		}

		public AppUser CurrentUser { 
			get {
				return null;
			} }
	}
}
