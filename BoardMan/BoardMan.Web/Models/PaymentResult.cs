using BoardMan.Web.Data;
using Microsoft.AspNetCore.Identity;

namespace BoardMan.Web.Models
{
	public class PaymentResult
	{
		public PaymentStatus PaymentStatus { get; set; }

		public UserResult UserDetails { get; set; }
	}

	public class UserResult
	{
		public AppUser User { get; set; }

		public IdentityResult CreateResult { get; set; }

		public bool UserCreated => User != null;

		public bool UserIsLoggedIn { get; set; }
	}
}
