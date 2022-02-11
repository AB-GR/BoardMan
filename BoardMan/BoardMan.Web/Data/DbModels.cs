using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BoardMan.Web.Data
{
	public abstract class DbEntity
	{
		[Key]
		public Guid Id { get; set; }

		public DateTime CreatedAt { get; set; }

		public DateTime ModifiedAt { get; set; }
	}

	public enum PlanType
	{
		Monthly,
		Annual
	}

	public enum PaymentStatus
	{
		Success,
		Failed,
		InProgress
	}

	[Table("Subscriptions")]
	public class DbSubscription : DbEntity
	{
		[MaxLength(100)]
		public string Name { get; set; }

		[Required]
		public DateTime StartedAt { get; set; }

		[Required]
		public DateTime ExpireAt { get; set; }

		[NotMapped]
		public bool Expired => ExpireAt <= DateTime.UtcNow;

		[ForeignKey("PaymentTrasaction")]
		public Guid PaymentTrasactionId { get; set; }

		public DbPaymentTrasaction PaymentTrasaction { get; set; }


		[ForeignKey("AppUser")]
		public Guid UserId { get; set; }

		public AppUser AppUser { get; set; }

		public DateTime? DeletedAt { get; set; }
	}

	[Table("Plans")]
	public class DbPlan : DbEntity
	{
		[MaxLength(100)]
		public string Name { get; set; }

		[MaxLength(256)]
		public string Description { get; set; }

		[Required]
		public decimal Cost { get; set; }

		[MaxLength(3)]
		public string Currency { get; set; }

		[Required]
		public PlanType PlanType { get; set; }

		public DateTime? ExpireAt { get; set; }

		[NotMapped]
		public bool Expired => ExpireAt.HasValue && ExpireAt <= DateTime.UtcNow;

		public DateTime? DeletedAt { get; set; }

	}

	[Table("PlanDiscounts")]
	public class DbPlanDiscount : DbEntity
	{
		[MaxLength(256)]
		public string Message { get; set; }

		[Required]
		[Range(0, 100)]
		public decimal DiscountPercent { get; set; }

		[Required]
		[MaxLength(10)]
		public string Code { get; set; }

		public DateTime? ExpireAt { get; set; }

		[NotMapped]
		public bool Expired => ExpireAt.HasValue && ExpireAt <= DateTime.UtcNow;

		[ForeignKey("Plan")]		
		public Guid PlanId { get; set; }

		public DbPlan Plan { get; set; }

		public DateTime? DeletedAt { get; set; }
	}

	[Table("PaymentTransactions")]
	public class DbPaymentTrasaction : DbEntity
	{
		[Required]
		public PaymentStatus Status { get; set; }

		public string StatusReason { get; set; }

		[ForeignKey("Plan")]
		public Guid PlanId { get; set; }

		public DbPlan Plan { get; set; }

		[ForeignKey("PlanDiscount")]
		public Guid PlanDiscountId { get; set; }

		public DbPlanDiscount PlanDiscount { get; set; }

		public decimal CostBeforeDiscount { get; set; }

		public decimal DiscountApplied { get; set; }

		[Required]
		public decimal FinalCost { get; set; }
	}

}
