using AutoMapper;
using BoardMan.Web.Data;
using BoardMan.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace BoardMan.Web.Managers
{
	public interface IRoleManager
	{
		Task<List<ComboOption>> ListRolesAsync(RoleType roleType, bool excludeSuperAdmin = true);
	}

	public class RoleManager : IRoleManager
	{
		private readonly BoardManDbContext context;

		public RoleManager(BoardManDbContext context)
		{
			this.context = context;
		}		

		public async Task<List<ComboOption>> ListRolesAsync(RoleType roleType, bool excludeSuperAdmin = true)
		{
			var dbRoles = await this.context.Roles.Where(x => x.RoleType == roleType x.Name.Contains("SuperAdmin")).ToListAsync().ConfigureAwait(false);
			return dbRoles.Select(x => new ComboOption { Value = x.Id, DisplayText = x.Name }).ToList();
		}
	}
}
