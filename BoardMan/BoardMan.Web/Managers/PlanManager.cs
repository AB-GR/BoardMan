﻿using AutoMapper;
using BoardMan.Web.Data;
using BoardMan.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace BoardMan.Web.Managers
{
	public interface IPlanManager
	{
		Task<List<PlanVM>> GetAllPlansAsync(bool includeExpired = false);
	}

	public class PlanManager : IPlanManager
	{
		private readonly BoardManDbContext _dbContext;
		private readonly IMapper mapper;

		public PlanManager(BoardManDbContext dbContext, IMapper mapper)
		{
			_dbContext = dbContext;
			this.mapper = mapper;
		}

		public async Task<List<PlanVM>> GetAllPlansAsync(bool includeExpired = false)
		{
			var dbPlans = await _dbContext.Plans.ToListAsync().ConfigureAwait(false);
			return mapper.Map<List<PlanVM>>(dbPlans.Where(p => p.Expired == includeExpired));
		}
	}
}
