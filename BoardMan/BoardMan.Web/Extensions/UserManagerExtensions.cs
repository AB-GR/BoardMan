using BoardMan.Web.Data;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace BoardMan.Web.Extensions
{
	public static class UserManagerExtensions
	{
		public static Guid GetGuidUserId(this UserManager<AppUser> userManager, ClaimsPrincipal user)
		{			
			var userId = userManager.GetUserId(user);
			return Guid.TryParse(userId, out Guid result) ? result : Guid.Empty;
		}
	}
}
