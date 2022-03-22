using System.ComponentModel.DataAnnotations;

namespace BoardMan.Web.Models
{
	public class EmailInviteModel
	{
		public string Token { get; set; }

		public bool? IsTokenValid { get; set; }

		public string? ValidationMessage { get; set; }

		[Display(Name = "First Name")]
        public string FirstName { get; set; }
        
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
	}
}
