using BoardMan.Web.Infrastructure.Utils.Extensions;
using System.ComponentModel.DataAnnotations;

namespace BoardMan.Web.Models
{
	public class ValidatePaymentRequest : IValidatableObject
	{
		public Guid? UserId { get; set; }

		public string? UserEmail { get; set; }

		public Guid PlanId { get; set; }

		public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
		{
			if(UserId.IsNullOrEmpty() && string.IsNullOrEmpty(UserEmail))
			{
				yield return new ValidationResult("Either UserId or UserEmail should be present", new[] {nameof(UserId), nameof(UserEmail)});
			}
		}
	}
}
