using BoardMan.Web.Data;

namespace BoardMan.Web.Models
{
	public class BoardMember
	{
		public Guid? Id { get; set; }

		public Guid? BoardId { get; set; }

		public string Username { get; set; }

		public MemberStatus? Status { get; set; }
	}
}
