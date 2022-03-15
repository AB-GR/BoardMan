namespace BoardMan.Web.Models
{
	public class TaskWatcher
	{
		public Guid Id { get; set; }

		public Guid TaskId { get; set; }

		public Guid? WatchedById { get; set; }
	}
}
