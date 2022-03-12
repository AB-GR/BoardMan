namespace BoardMan.Web.Models
{
	public class Workspace
	{
		public Guid Id { get; set; }

		public string Title { get; set; }

		public string Description { get; set; }

		public bool HasSubscription { get; set; }

		public string? SubscriptionName { get; set; }

		public List<Board> Boards { get; set; }
	}
}
