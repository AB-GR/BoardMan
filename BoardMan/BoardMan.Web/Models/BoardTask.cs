using System.ComponentModel.DataAnnotations;

namespace BoardMan.Web.Models
{
	public class BoardTask
	{
		public Guid Id { get; set; }

		[MaxLength(100)]
		public string Title { get; set; } = null!;

		[MaxLength(250)]
		public string? Description { get; set; }

		public DateTime? EndDate { get; set; }

		public DateTime? ActualEndDate { get; set; }

		public bool? IsCompleted { get; set; }

		public Guid ListId { get; set; }
	}
}
