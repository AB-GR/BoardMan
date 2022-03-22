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
		private readonly IMapper mapper;

		public RoleManager(BoardManDbContext context, IMapper mapper)
		{
			this.context = context;
			this.mapper = mapper;
		}		

		public async Task<List<ComboOption>> ListRolesAsync()
		{
			var dbRoles = await this.context.BoardRoles.ToListAsync();
			return this.mapper.Map<List<BoardmanRole>>(dbRoles).Select(x => new ComboOption { Value = x.Id, DisplayText = x.Name }).ToList();
		}
	}
}
