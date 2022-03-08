using AutoMapper;
using BoardMan.Web.Data;
using BoardMan.Web.Models;

namespace BoardMan.Web
{
    public class WebProfile : Profile
    {
        public WebProfile()
        {
            CreateMap<DbPlan, Plan>();
            CreateMap<PaymentIntentTransaction, DbPaymentTransaction>()
                .ForMember(x => x.Plan, x => x.Ignore())
                .ForMember(x => x.PlanDiscount, x => x.Ignore())
                .ForMember(x => x.TransactedBy, x => x.Ignore())
                .ForMember(x => x.CreatedAt, x => x.Ignore())
                .ForMember(x => x.ModifiedAt, x => x.Ignore())
                .ForMember(x => x.DeletedAt, x => x.Ignore());

            CreateMap<BillingDetails, DbBillingDetails>()
                .ForMember(x => x.PaymentTransactionId, x => x.Ignore())
                .ForMember(x => x.PaymentTransaction, x => x.Ignore())
                .ForMember(x => x.CreatedAt, x => x.Ignore())
                .ForMember(x => x.ModifiedAt, x => x.Ignore())
                .ForMember(x => x.DeletedAt, x => x.Ignore());
        }
    }

}
