using BoardMan.Web.Data;
using System.ComponentModel.DataAnnotations;

namespace BoardMan.Web.Models
{
	public class TaskAttachment
	{
		public Guid? Id { get; set; }

		public Guid TaskId { get; set; }

		[MaxLength(50)]
		public string? TrustedFileName { get; set; } = null!;

		public string? FileUri { get; set; } = null!;

		public string? Note { get; set; }

		public long? Size { get; set; }

		public Guid? UploadedById { get; set; }

		public string? UploadedByName { get; set; }

		public DateTime? CreatedAt { get; set; }

		public IFormFile File { get; set; }
	}
}
