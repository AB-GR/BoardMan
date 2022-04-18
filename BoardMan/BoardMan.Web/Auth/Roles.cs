namespace BoardMan.Web.Auth
{
	public class Roles
	{
		public const string WorkspaceReader = "WorkspaceReader";

		public static readonly Guid WorkspaceReaderId = new Guid("dcf8ad71-84fc-4156-bc8a-87a7919e9411");

		public const string WorkspaceContributor = "WorkspaceContributor";

		public static readonly Guid WorkspaceContributorId = new Guid("9b6b04d5-6c29-4c75-bb3c-40ac6707f4a5");

		public const string WorkspaceAdmin = "WorkspaceAdmin";

		public static readonly Guid WorkspaceAdminId = new Guid("981688cb-3f98-4c15-b32a-e7b0ef4d71f7");

		public const string WorkspaceSuperAdmin = "WorkspaceSuperAdmin";

		public static readonly Guid WorkspaceSuperAdminId = new Guid("25783338-d4e3-4764-b2bd-6e59b8d7060a");

		public const string BoardReader = "BoardReader";

		public static readonly Guid BoardReaderId = new Guid("92ca9848-971a-4cae-bb00-2381684446d1");

		public const string BoardContributor = "BoardContributor";

		public static readonly Guid BoardContributorId = new Guid("ccdb6d85-d24b-423a-9141-989f7f3b376b");

		public const string BoardAdmin = "BoardAdmin";

		public static readonly Guid BoardAdminId = new Guid("9a4b4b1c-91ff-4cc6-9ba1-af7f2c97fd96");

		public const string BoardSuperAdmin = "BoardSuperAdmin";

		public static readonly Guid BoardSuperAdminId = new Guid("8c4fcb08-396b-4298-8bbd-15ebeda8db4b");

		public const string ApplicationSuperAdmin = "ApplicationSuperAdmin";

		public static readonly Guid ApplicationSuperAdminId = new Guid("49e3045d-b948-4d0a-b596-455673bd989c");
	}
}
