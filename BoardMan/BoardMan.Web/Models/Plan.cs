using BoardMan.Web.Data;
using System.ComponentModel.DataAnnotations;

namespace BoardMan.Web.Models
{
	public class Plan
	{
		public Guid? Id { get; set; }

		[MaxLength(100)]
		public string Name { get; set; }

		[MaxLength(256)]
		public string? Description { get; set; }
				
		public decimal Cost { get; set; }

		[MaxLength(3)]
		public string Currency { get; set; }
				
		public PlanType PlanType { get; set; }

		public DateTime? ExpireAt { get; set; }
				
		public bool Expired => ExpireAt.HasValue && ExpireAt <= DateTime.UtcNow;

		public DateTime? DeletedAt { get; set; }

		public DateTime? CreatedAt { get; set; }

		public DateTime? ModifiedAt { get; set; }
	}
}
