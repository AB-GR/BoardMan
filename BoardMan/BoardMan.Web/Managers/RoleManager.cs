using AutoMapper;
using BoardMan.Web.Data;
using BoardMan.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace BoardMan.Web.Managers
{
	public interface IRoleManager
	{
		Task<List<ComboOption>> ListRolesAsync();
	}

	public class RoleManager : IRoleManager
	{
		private readonly BoardManDbContext context;

		public RoleManager(BoardManDbContext context)
		{
			this.context = context;
		}		

		public async Task<List<ComboOption>> ListRolesAsync()
		{
			var dbRoles = await this.context.Roles.ToListAsync();
			return dbRoles.Select(x => new ComboOption { Value = x.Id, DisplayText = x.Name }).ToList();
		}
	}
}
