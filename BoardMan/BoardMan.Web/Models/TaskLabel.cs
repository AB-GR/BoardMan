using System.ComponentModel.DataAnnotations;

namespace BoardMan.Web.Models
{
	public class TaskLabel
	{
		public Guid Id { get; set; }

		[MaxLength(30)]
		public string Label { get; set; } = null!;

		public Guid TaskId { get; set; }
	}
}
