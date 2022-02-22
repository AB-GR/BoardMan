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

	[Table("Workspaces")]
	public class DbWorkspace : DbEntity
	{
		[MaxLength(100)]
		public string Title { get; set; }

		[MaxLength(250)]
		public string? Description { get; set; }

		[ForeignKey("Subscription")]
		public Guid SubscriptionId { get; set; }

		public DbSubscription Subscription { get; set; }

		[ForeignKey("Owner")]
		public Guid OwnerId { get; set; }

		public AppUser Owner { get; set; }
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

		[Required]
		public decimal FinalCost { get; set; }

		[MaxLength(3)]		
		public string Currency { get; set; }

		[ForeignKey("TransactedBy")]
		public Guid TransactedById { get; set; }

		public AppUser TransactedBy { get; set; }

		public DbBillingDetails BillingDetails { get; set; }
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
