using BoardMan.Web.Infrastructure.Utils.Extensions;
using BoardMan.Web.Infrastructure.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace BoardMan.Web.Models
{
	public class BuyPlan : IValidatableObject
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

		public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		{
			if(UserId.IsNullOrEmpty() && (BillingDetails == null || string.IsNullOrWhiteSpace(BillingDetails.UserEmail) || string.IsNullOrWhiteSpace(BillingDetails.UserFirstName) || string.IsNullOrWhiteSpace(BillingDetails.UserLastName) || string.IsNullOrWhiteSpace(BillingDetails.Password)))
			{
				yield return new ValidationResult("For buying a subscription, either login or provide the details to register a new user", new[] {nameof(UserId), nameof(BillingDetails.UserEmail), nameof(BillingDetails.UserFirstName), nameof(BillingDetails.UserLastName), nameof(BillingDetails.Password) });
			}
		}
	}

	public class BillingDetails
	{
		[Required]
		[Display(Name = "First Name")]
		[MaxLength(50)]
		public string UserFirstName { get; set; }

		[Required]
		[Display(Name = "Last Name")]
		[MaxLength(50)]
		public string UserLastName { get; set; }

		[Required]
		[EmailAddress]
		[Display(Name = "Email")]
		public string UserEmail { get; set; }

		public bool? IsAnonymousUser { get; set; }

		[RequiredIfTrue(nameof(IsAnonymousUser), ErrorMessage = "The Password field is required.")]
		[StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
		[DataType(DataType.Password)]
		[Display(Name = "Password")]
		public string? Password { get; set; }

		[DataType(DataType.Password)]
		[Display(Name = "Confirm password")]
		[Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
		public string? ConfirmPassword { get; set; }

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
		public string? AddressLine2 { get; set; }

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
