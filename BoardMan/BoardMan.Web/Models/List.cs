using System.ComponentModel.DataAnnotations;

namespace BoardMan.Web.Models
{
	public class List
	{
		public Guid Id { get; set; }

		[MaxLength(100)]
		public string Title { get; set; } = null!;

		[MaxLength(250)]
		public string? Description { get; set; }

		public Guid BoardId { get; set; }
	}
}
