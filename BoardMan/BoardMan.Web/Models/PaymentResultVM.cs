using BoardMan.Web.Data;
using Microsoft.AspNetCore.Identity;

namespace BoardMan.Web.Models
{
	public class PaymentResultVM
	{
		public PaymentStatus PaymentStatus { get; set; }

		public UserResultVM NewUser { get; set; }
	}

	public class UserResultVM
	{
		public AppUser User { get; set; }

		public IdentityResult CreateResult { get; set; }

		public bool Created => User != null;
	}
}
