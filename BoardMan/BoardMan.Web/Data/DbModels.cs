using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BoardMan.Web.Data
{
	public abstract class DbEntity
	{
		[Key]
		public Guid Id { get; set; }

		public DateTime? CreatedAt { get; set; }

		public DateTime? ModifiedAt { get; set; }

		public DateTime? DeletedAt { get; set; }
	}

	public enum SqlErrors
	{		
		UniqueIndex = 2601,
		
		KeyViolation = 2627,
		
		UserDefined = 50000
	}

	public enum PlanType
	{
		Monthly,
		Annual
	}

	public enum PaymentStatus
	{
		Processed,
		Failed,
		CanBeProcessed,
		Invalid,
		PlanExpired,
		AmountNotMatched
	}

	public enum AttachmentType
	{
		Document,
		Audio,
		Video
	}

	public enum EntityType
	{
		Workspace,
		Board,
		Task
	}

	public enum UserAction
	{
		None
	}

	public enum MemberStatus
	{
		Confirmed,
		InviteSent,
		InviteRejected, 
		InviteAccepted,
		InviteExpired,
		ExistingUser
	}

	[Table("Workspaces")]
	public class DbWorkspace : DbEntity
	{
		[MaxLength(100)]
		public string Title { get; set; } = null!;

		[MaxLength(250)]
		public string? Description { get; set; }

		[ForeignKey("Subscription")]
		public Guid? SubscriptionId { get; set; }

		public DbSubscription? Subscription { get; set; } = null!;

		[ForeignKey("Owner")]
		public Guid OwnerId { get; set; }

		public AppUser Owner { get; set; } = null!;

		public List<DbBoard> Boards { get; set; } = null!;
	}

	[Table("WorkspaceMembers")]
	public class DbWorkspaceMember : DbEntity
	{
		//  Include role as well

		[ForeignKey("Workspace")]
		public Guid WorkspaceId { get; set; }

		public DbWorkspace Workspace { get; set; } = null!;

		[ForeignKey("Member")]
		public Guid MemberId { get; set; }

		public AppUser Member { get; set; } = null!;
	}

	[Table("Boards")]
	public class DbBoard : DbEntity
	{
		[MaxLength(100)]
		public string Title { get; set; } = null!;

		[MaxLength(250)]
		public string? Description { get; set; }

		[ForeignKey("Workspace")]
		public Guid WorkspaceId { get; set; }

		public DbWorkspace Workspace { get; set; } = null!;

		[ForeignKey("Owner")]
		public Guid OwnerId { get; set; }

		public AppUser Owner { get; set; } = null!;
	}

	[Table("BoardMembers")]
	public class DbBoardMember : DbEntity
	{
		//  Include role as well

		[ForeignKey("Board")]
		public Guid BoardId { get; set; }

		public DbBoard Board { get; set; } = null!;

		[ForeignKey("Member")]
		public Guid MemberId { get; set; }

		public AppUser Member { get; set; } = null!;
	}


	[Table("Lists")]
	public class DbList : DbEntity
	{
		[MaxLength(100)]
		public string Title { get; set; } = null!;

		[MaxLength(250)]
		public string? Description { get; set; }

		[ForeignKey("Board")]
		public Guid BoardId { get; set; }

		public DbBoard Board { get; set; } = null!;
	}

	[Table("Tasks")]
	public class DbTask : DbEntity
	{
		[MaxLength(100)]
		public string Title { get; set; } = null!;

		[MaxLength(250)]
		public string? Description { get; set; }

		public DateTime? EndDate { get; set; }

		public DateTime? ActualEndDate { get; set; }

		public bool? IsCompleted { get; set; }

		[ForeignKey("List")]
		public Guid ListId { get; set; }

		public DbList List { get; set; } = null!;

		[ForeignKey("AssignedTo")]
		public Guid? AssignedToId { get; set; }

		public AppUser? AssignedTo { get; set; } = null!;
	}

	[Table("TaskComments")]
	public class DbTaskComment : DbEntity
	{
		[ForeignKey("Task")]
		public Guid TaskId { get; set; }

		public DbTask Task { get; set; } = null!;

		[MaxLength(250)]
		public string Comment { get; set; } = null!;

		[ForeignKey("CommentedBy")]
		public Guid CommentedById { get; set; }

		public AppUser CommentedBy { get; set; } = null!;
	}

	[Table("TaskChecklists")]
	public class DbTaskChecklist : DbEntity
	{
		[MaxLength(250)]
		public string Description { get; set; } = null!;

		public bool? IsComplete { get; set; }

		public int? Priority { get; set; }

		[ForeignKey("Task")]
		public Guid TaskId { get; set; }

		public DbTask Task { get; set; } = null!;

		[ForeignKey("CommentedBy")]
		public Guid CreatedById { get; set; }

		public AppUser CreatedBy { get; set; } = null!;
	}

	[Table("TaskAttachments")]
	public class DbTaskAttachment : DbEntity
	{
		[MaxLength(50)]
		public string TrustedFileName { get; set; } = null!;

		public string FileUri { get; set; } = null!;

		public string? Note { get; set; }

		public long Size { get; set; }

		[ForeignKey("Task")]
		public Guid TaskId { get; set; }

		public DbTask Task { get; set; } = null!;

		[ForeignKey("UploadedBy")]
		public Guid UploadedById { get; set; }

		public AppUser UploadedBy { get; set; } = null!;
	}

	[Table("TaskWatchers")]
	public class DbTaskWatcher : DbEntity
	{
		[ForeignKey("Task")]
		public Guid TaskId { get; set; }

		public DbTask Task { get; set; } = null!;

		[ForeignKey("WatchedBy")]
		public Guid WatchedById { get; set; }

		public AppUser WatchedBy { get; set; } = null!;
	}

	[Table("TaskLabels")]
	public class DbTaskLabel : DbEntity
	{
		[MaxLength(30)]
		public string Label { get; set; } = null!;

		[ForeignKey("Task")]
		public Guid TaskId { get; set; }

		public DbTask Task { get; set; } = null!;
	}

	[Table("ActivityTrackings")]
	public class DbActivityTracking : DbEntity
	{
		public string EntityUrn { get; set; } = null!;

		public string PropertyName { get; set; } = null!;

		public string? OldValue { get; set; }

		public string NewValue { get; set; } = null!;

		public UserAction Action { get; set; }

		[ForeignKey("DoneBy")]
		public Guid DoneById { get; set; }

		public AppUser DoneBy { get; set; } = null!;
	}

	[Table("EmailInvites")]
	public class DbEmailInvite : DbEntity
	{
		//  Include prospective role as well

		public string EntityUrn { get; set; } = null!;

		public string EmailAddress { get; set; } = null!;

		public string Token { get; set; } = null!;

		public DateTime ExpireAt { get; set; }

		public bool? Accepted { get; set; }
	}

	[Table("Subscriptions")]
	public class DbSubscription : DbEntity
	{
		[MaxLength(100)]
		public string Name { get; set; }
				
		public DateTime StartedAt { get; set; }

		public DateTime ExpireAt { get; set; }

		[NotMapped]
		public bool Expired => ExpireAt <= DateTime.UtcNow;

		[ForeignKey("PaymentTrasaction")]
		public Guid PaymentTrasactionId { get; set; }

		public DbPaymentTransaction PaymentTrasaction { get; set; }

		[ForeignKey("AppUser")]
		public Guid OwnerId { get; set; }

		public AppUser Owner { get; set; }
	}

	[Table("Plans")]
	public class DbPlan : DbEntity
	{
		[MaxLength(100)]
		public string Name { get; set; }

		[MaxLength(256)]
		public string? Description { get; set; }
				
		public decimal Cost { get; set; }

		[MaxLength(3)]
		public string Currency { get; set; }
				
		public PlanType PlanType { get; set; }

		public DateTime? ExpireAt { get; set; }

		[NotMapped]
		public bool Expired => ExpireAt.HasValue && ExpireAt <= DateTime.UtcNow;

	}

	[Table("PlanDiscounts")]
	public class DbPlanDiscount : DbEntity
	{
		[MaxLength(256)]
		public string? Message { get; set; }
				
		[Range(0, 100)]
		public decimal DiscountPercent { get; set; }
				
		[MaxLength(10)]
		public string Code { get; set; }

		public DateTime? ExpireAt { get; set; }

		[NotMapped]
		public bool Expired => ExpireAt.HasValue && ExpireAt <= DateTime.UtcNow;

		[ForeignKey("Plan")]		
		public Guid PlanId { get; set; }

		public DbPlan Plan { get; set; }
	}

	[Table("PaymentTransactions")]
	public class DbPaymentTransaction : DbEntity
	{		
		public string PaymentReference { get; set; }
				
		public PaymentStatus Status { get; set; }

		public string StatusReason { get; set; }

		[ForeignKey("Plan")]
		public Guid PlanId { get; set; }

		public DbPlan Plan { get; set; }

		[ForeignKey("PlanDiscount")]
		public Guid? PlanDiscountId { get; set; }

		public DbPlanDiscount? PlanDiscount { get; set; }

		public decimal CostBeforeDiscount { get; set; }

		public decimal DiscountApplied { get; set; }
				
		public decimal FinalCost { get; set; }

		[MaxLength(3)]		
		public string Currency { get; set; }

		[ForeignKey("TransactedBy")]
		public Guid? TransactedById { get; set; }

		public AppUser? TransactedBy { get; set; }

		public DbBillingDetails BillingDetails { get; set; }

		public string RawData { get; set; }
	}

	[Table("BillingDetails")]
	public class DbBillingDetails : DbEntity
	{		
		[MaxLength(50)]
		public string UserFirstName { get; set; }
				
		[MaxLength(50)]
		public string UserLastName { get; set; }
				
		public string UserEmail { get; set; }
				
		[MaxLength(100)]
		public string NameAsOnCard { get; set; }
				
		[MaxLength(200)]
		public string AddressLine1 { get; set; }

		[MaxLength(200)]
		public string? AddressLine2 { get; set; }
				
		[MaxLength(50)]
		public string City { get; set; }
				
		[MaxLength(50)]
		public string State { get; set; }
				
		[MaxLength(20)]
		public string ZipCode { get; set; }
				
		[MaxLength(3)]
		public string Country { get; set; }

		[ForeignKey("PaymentTransaction")]
		public Guid PaymentTransactionId { get; set; }

		public DbPaymentTransaction PaymentTransaction { get; set; }
	}
}
