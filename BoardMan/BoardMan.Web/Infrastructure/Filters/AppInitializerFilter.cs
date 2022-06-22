using BoardMan.Web.Data;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace BoardMan.Web.Infrastructure.Filters
{
	public class AppInitializerFilter : IAsyncActionFilter
	{
		private BoardManDbContext dbContext;

		public AppInitializerFilter(BoardManDbContext dbContext)
		{
			this.dbContext = dbContext;
		}

		public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
		{
			var claimsIdentity = (ClaimsIdentity?)context.HttpContext.User.Identity;

			if (claimsIdentity != null)
			{
				var userIdClaim = claimsIdentity.Claims.SingleOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
				if (userIdClaim != null)
				{
					this.dbContext.LoggedInUserId = Guid.Parse(userIdClaim.Value);
				}
			}

			await next();
		}
	}
}
