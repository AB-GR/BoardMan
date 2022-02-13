using AutoMapper;
using BoardMan.Web.Data;
using BoardMan.Web.Models;

namespace BoardMan.Web
{
    public class WebProfile : Profile
    {
        public WebProfile()
        {
            CreateMap<DbPlan, PlanVM>();
        }
    }

}
