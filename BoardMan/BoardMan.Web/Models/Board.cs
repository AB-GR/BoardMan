namespace BoardMan.Web.Models
{
	public class Board
	{
		public Guid Id { get; set; }

		public Guid WorkspaceId { get; set; }

		public string Title { get; set; }

		public string Description { get; set; }
	}
}
