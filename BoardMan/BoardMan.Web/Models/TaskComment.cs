using System.ComponentModel.DataAnnotations;

namespace BoardMan.Web.Models
{
	public class TaskComment
	{
		public Guid Id { get; set; }

		public Guid TaskId { get; set; }

		[MaxLength(250)]
		public string Comment { get; set; } = null!;

		public Guid? CommentedById { get; set; }

		public string? CommentedByName { get; set; }
	}
}
