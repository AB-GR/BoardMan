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

            CreateMap<DbTaskChecklist, TaskChecklist>();
            CreateMap<TaskChecklist, DbTaskChecklist>();

            CreateMap<DbTaskWatcher, TaskWatcher>();
            CreateMap<TaskWatcher, DbTaskWatcher>();

            CreateMap<DbTaskAttachment, TaskAttachment>()
                .ForMember(x => x.File, x => x.Ignore())
                .ForMember(x => x.UploadedByName, x => x.MapFrom(src => src.UploadedBy.UserName));
            CreateMap<TaskAttachment, DbTaskAttachment>()
                .ForMember(x => x.CreatedAt, x => x.Ignore())
                .ForMember(x => x.Size, x => x.MapFrom(src => src.File.Length));

            CreateMap<DbBoardMember, BoardMember>()
                .ForMember(x => x.MemberEmail, x => x.MapFrom(src => src.Member.UserName))
                .ForMember(x => x.MemberName, x => x.MapFrom(src => src.Member.FirstName + " " + src.Member.LastName))
                .ForMember(x => x.Status, x => x.MapFrom(src => MemberStatus.ExistingUser))
                .ForMember(x => x.IsAMember, x => x.MapFrom(src => true))
                .ForMember(x => x.AddedByName, x => x.MapFrom(src => src.AddedBy.UserName))
                .ForMember(x => x.AddedById, x => x.MapFrom(src => src.AddedBy.Id));

            CreateMap<DbEmailInvite, BoardMember>()
                .ForMember(x => x.MemberEmail, x => x.MapFrom(src => src.EmailAddress))
                .ForMember(x => x.BoardId, x => x.MapFrom(src => src.EntityUrn.Contains("Board:") ? Guid.Parse(src.EntityUrn.Substring(src.EntityUrn.IndexOf(":") + 1)) : (Guid?)null))
                .ForMember(x => x.MemberName, x => x.Ignore())
                .ForMember(x => x.MemberId, x => x.Ignore())
                .ForMember(x => x.Status, x => x.MapFrom(src => src.ExpireAt < DateTime.UtcNow ? MemberStatus.InviteExpired : (src.Accepted.GetValueOrDefault() ? MemberStatus.InviteAccepted : MemberStatus.InviteSent)))
                .ForMember(x => x.IsAMember, x => x.MapFrom(src => false))
                .ForMember(x => x.AddedByName, x => x.MapFrom(src => src.AddedBy.UserName))
                .ForMember(x => x.AddedById, x => x.MapFrom(src => src.AddedBy.Id));

            CreateMap<BoardMember, DbBoardMember>()
                .ForMember(x => x.CreatedAt, x => x.Ignore())
                .ForMember(x => x.ModifiedAt, x => x.Ignore())
                .ForMember(x => x.DeletedAt, x => x.Ignore());

            CreateMap<BoardMember, DbEmailInvite>()
                .ForMember(x => x.EmailAddress, x => x.MapFrom(src => src.MemberEmail))
                .ForMember(x => x.EntityUrn, x => x.MapFrom(src => $"Board:{src.BoardId}"))
                .ForMember(x => x.Accepted, x => x.Ignore())
                .ForMember(x => x.Token, x => x.Ignore())
                .ForMember(x => x.ExpireAt, x => x.Ignore())
                .ForMember(x => x.CreatedAt, x => x.Ignore())
                .ForMember(x => x.ModifiedAt, x => x.Ignore())
                .ForMember(x => x.DeletedAt, x => x.Ignore());


            CreateMap<DbWorkspaceMember, WorkspaceMember>()
                .ForMember(x => x.MemberEmail, x => x.MapFrom(src => src.Member.UserName))
                .ForMember(x => x.MemberName, x => x.MapFrom(src => src.Member.FirstName + " " + src.Member.LastName))
                .ForMember(x => x.Status, x => x.MapFrom(src => MemberStatus.ExistingUser))
                .ForMember(x => x.IsAMember, x => x.MapFrom(src => true))
                .ForMember(x => x.AddedByName, x => x.MapFrom(src => src.AddedBy.UserName))
                .ForMember(x => x.AddedById, x => x.MapFrom(src => src.AddedBy.Id));

            CreateMap<DbEmailInvite, WorkspaceMember>()
                .ForMember(x => x.MemberEmail, x => x.MapFrom(src => src.EmailAddress))
                .ForMember(x => x.WorkspaceId, x => x.MapFrom(src => src.EntityUrn.Contains("Workspace:") ? Guid.Parse(src.EntityUrn.Substring(src.EntityUrn.IndexOf(":") + 1)) : (Guid?)null))
                .ForMember(x => x.MemberName, x => x.Ignore())
                .ForMember(x => x.MemberId, x => x.Ignore())
                .ForMember(x => x.Status, x => x.MapFrom(src => src.ExpireAt < DateTime.UtcNow ? MemberStatus.InviteExpired : (src.Accepted.GetValueOrDefault() ? MemberStatus.InviteAccepted : MemberStatus.InviteSent)))
                .ForMember(x => x.IsAMember, x => x.MapFrom(src => false))
                .ForMember(x => x.AddedByName, x => x.MapFrom(src => src.AddedBy.UserName))
                .ForMember(x => x.AddedById, x => x.MapFrom(src => src.AddedBy.Id));

            CreateMap<WorkspaceMember, DbWorkspaceMember>()
                .ForMember(x => x.CreatedAt, x => x.Ignore())
                .ForMember(x => x.ModifiedAt, x => x.Ignore())
                .ForMember(x => x.DeletedAt, x => x.Ignore());

            CreateMap<WorkspaceMember, DbEmailInvite>()
                .ForMember(x => x.EmailAddress, x => x.MapFrom(src => src.MemberEmail))
                .ForMember(x => x.EntityUrn, x => x.MapFrom(src => $"Workspace:{src.WorkspaceId}"))
                .ForMember(x => x.Accepted, x => x.Ignore())
                .ForMember(x => x.Token, x => x.Ignore())
                .ForMember(x => x.ExpireAt, x => x.Ignore())
                .ForMember(x => x.CreatedAt, x => x.Ignore())
                .ForMember(x => x.ModifiedAt, x => x.Ignore())
                .ForMember(x => x.DeletedAt, x => x.Ignore());
        }
    }

}
