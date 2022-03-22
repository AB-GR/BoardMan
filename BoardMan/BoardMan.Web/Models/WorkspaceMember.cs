using BoardMan.Web.Data;

namespace BoardMan.Web.Models
{
	public class WorkspaceMember
	{
		public Guid? Id { get; set; }

		public Guid? WorkspaceId { get; set; }

		public string MemberEmail { get; set; } = null!;

		public Guid? MemberId { get; set; }

		public string? MemberName { get; set; }

		public Guid RoleId { get; set; }

		public MemberStatus? Status { get; set; }

		public DateTime? CreatedAt { get; set; }

		public bool? IsAMember { get; set; }

		public Guid? AddedById { get; set; }

		public string? AddedByName { get; set; }
	}
}
