using AutoMapper;
using BoardMan.Web.Auth;
using BoardMan.Web.Data;
using BoardMan.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;

namespace BoardMan.Web.Controllers
{
	[Authorize(Roles = Roles.ApplicationSuperAdmin)]
	public class AdminController : SiteControllerBase
	{
		private readonly BoardManDbContext dbContext;
		private readonly IMapper mapper;

		public AdminController(BoardManDbContext dbContext, IMapper mapper, UserManager<DbAppUser> userManager, IAuthorizationService authorizationService, IConfiguration configuration, ILogger<AdminController> logger, IStringLocalizer<SharedResource> sharedLocalizer) : base(userManager, authorizationService, configuration, logger, sharedLocalizer)
		{
			this.dbContext = dbContext;
			this.mapper = mapper;
		}

		public IActionResult Index()
		{			
			return View();
		}

		[HttpPost]
		public async Task<ActionResult> ListUsers(string name = "")
		{
			var dbUsers = await this.dbContext.Users.Where(x => x.Id != Users.ApplicationSuperAdminId && (string.IsNullOrWhiteSpace(name) || !string.IsNullOrWhiteSpace(name) && x.UserName.Contains(name))).ToListAsync();
			return JsonResponse(ApiResponse.List(this.mapper.Map<List<AppUser>>(dbUsers)));
		}

		[HttpPost]
		public async Task<ActionResult> ListUserSubscriptions(Guid userId)
		{
			var dbSubs = await this.dbContext.Subscriptions.Include(x => x.PaymentTrasaction.Plan).Where(x => x.OwnerId == userId).ToListAsync();
			return JsonResponse(ApiResponse.List(this.mapper.Map<List<Subscription>>(dbSubs)));
		}
	}
}
