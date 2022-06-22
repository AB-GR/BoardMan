namespace BoardMan.Web.Models
{
	public class AllWorkspaces
	{
		public Workspace Primary { get; set; }

		public List<Workspace> Others { get; set; }

		public string ReturnUrl { get; set; }
	}
}
