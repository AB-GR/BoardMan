using AutoMapper;
using BoardMan.Web.Data;
using BoardMan.Web.Infrastructure.Utils.Extensions;
using BoardMan.Web.Models;

namespace BoardMan.Web
{
    public class WebProfile : Profile
    {
        public WebProfile()
        {
            CreateMap<DbPlan, Plan>();
            
            CreateMap<DbSubscription, Subscription>()                
                .ForMember(x => x.PlanName, x => x.MapFrom(d => d.PaymentTrasaction.Plan.Name));
            
            CreateMap<DbPaymentTransaction, PaymentTransaction>()
                .ForMember(x => x.TransactedBy, x => x.MapFrom(d => d.TransactedBy.UserName));

            CreateMap<PaymentTransaction, DbPaymentTransaction>()
                .ForMember(x => x.Plan, x => x.Ignore())
                .ForMember(x => x.PlanDiscount, x => x.Ignore())
                .ForMember(x => x.TransactedBy, x => x.Ignore())
                .ForMember(x => x.CreatedAt, x => x.Ignore())
                .ForMember(x => x.ModifiedAt, x => x.Ignore())
                .ForMember(x => x.DeletedAt, x => x.Ignore());

            CreateMap<DbBillingDetails, BillingDetails>()
                .ForMember(x => x.IsAnonymousUser, x => x.Ignore())
                .ForMember(x => x.Password, x => x.Ignore())
                .ForMember(x => x.ConfirmPassword, x => x.Ignore());

            CreateMap<BillingDetails, DbBillingDetails>()
                .ForMember(x => x.PaymentTransactionId, x => x.Ignore())
                .ForMember(x => x.PaymentTransaction, x => x.Ignore())
                .ForMember(x => x.CreatedAt, x => x.Ignore())
                .ForMember(x => x.ModifiedAt, x => x.Ignore())
                .ForMember(x => x.DeletedAt, x => x.Ignore());

            CreateMap<DbWorkspace, Workspace>()
                .ForMember(x => x.HasSubscription, x => x.MapFrom((src, dest) => src.Subscription != null))
                .ForMember(x => x.SubscriptionName, x => x.MapFrom(src => src.Subscription != null ? src.Subscription.Name : string.Empty));

            CreateMap<DbBoard, Board>();
            CreateMap<Board, DbBoard>();

            CreateMap<DbList, List>();
            CreateMap<List, DbList>();

            CreateMap<DbTask, BoardTask>();
            CreateMap<BoardTask, DbTask>()
                .ForMember(x => x.AssignedToId, x => x.MapFrom(src => src.AssignedToId.IsNullOrEmpty() ? null : src.AssignedToId));

            CreateMap<DbTaskComment, TaskComment>()
                .ForMember(x => x.CommentedByName, x => x.MapFrom(src => src.CommentedBy.UserName));
            CreateMap<TaskComment, DbTaskComment>();

            CreateMap<DbTaskLabel, TaskLabel>();
            CreateMap<TaskLabel, DbTaskLabel>();
        }
    }

}
