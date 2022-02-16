using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace BoardMan.Web.Models
{
	public class BuyPlanVM
	{
		[Required]
		public Guid PlanId { get; set; }

		[Display(Name = "Plan")]
		public string PlanName { get; set; }

		[Display(Name = "Description")]
		public string PlanDescription { get; set; }

		public decimal Cost { get; set; }

		[Display(Name = "Cost")]
		public string CostDisplay => $"{Cost:0.##} {Currency}";

		public string Currency { get; set; }		

		public Guid? UserId { get; set; }

		public string PaymentKey { get; set; }

		public BillingDetails BillingDetails { get; set; }
	}

	public class BillingDetails
	{
		[Required]
		[Display(Name = "First Name")]
		[MaxLength(100)]
		public string UserFirstName { get; set; }

		[Required]
		[Display(Name = "Last Name")]
		[MaxLength(100)]
		public string UserLastName { get; set; }

		[Required]
		public string UserEmail { get; set; }

		[Required]
		[Display(Name = "Name as on Card")]
		[MaxLength(100)]
		public string NameAsOnCard { get; set; }

		[Required]
		[MaxLength(200)]
		[Display(Name = "Line1")]
		public string AddressLine1 { get; set; }

		[MaxLength(200)]
		[Display(Name = "Line2")]
		public string AddressLine2 { get; set; }

		[Required]
		[MaxLength(50)]		
		public string City { get; set; }

		[Required]
		[MaxLength(50)]
		public string State { get; set; }

		[Required]
		[MaxLength(20)]
		public string ZipCode { get; set; }

		[Required]
		[MaxLength(3)]
		public string Country => "US";

		public List<SelectListItem> Countries { get; } = new List<SelectListItem>
		{
			new SelectListItem { Value = "MX", Text = "Mexico" },
			new SelectListItem { Value = "CA", Text = "Canada" },
			new SelectListItem { Value = "US", Text = "USA"  },
		};
	}
}
