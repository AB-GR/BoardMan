using System.ComponentModel.DataAnnotations;

namespace BoardMan.Web.Models
{
	public class TaskChecklist
	{
		public Guid Id { get; set; }

		[MaxLength(30)]
		public string Description { get; set; } = null!;

		public bool? IsComplete { get; set; }

		public int? Priority { get; set; }

		public Guid TaskId { get; set; }

		public Guid? CreatedById { get; set; }
	}
}
