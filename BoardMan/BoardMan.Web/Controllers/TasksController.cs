using BoardMan.Web.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Localization;

namespace BoardMan.Web.Controllers
{
	public class TasksController : SiteControllerBase
	{
		public TasksController(UserManager<AppUser> userManager, IConfiguration configuration, ILogger logger, IStringLocalizer<SharedResource> sharedLocalizer) : base(userManager, configuration, logger, sharedLocalizer)
		{
		}
	}
}
