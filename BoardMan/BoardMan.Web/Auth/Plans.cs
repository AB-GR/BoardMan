namespace BoardMan.Web.Auth
{
	public class Plans
	{
		public const string FreePlan = "Free";

		public static readonly Guid FreePlanId = new Guid("281731b8-987c-442e-a3cb-79b9d5241e24");

		public const string StandardMonthly = "Standard (Monthly)";

		public static readonly Guid StandardMonthlyId = new Guid("4d1e3860-d571-4289-8c06-1fe31c892513");

		public const string StandardAnnual = "Standard (Annual)";

		public static readonly Guid StandardAnnualId = new Guid("e09f480c-62bd-4466-b3aa-0e221e3f7f4d");

		public const string PremiumMonthly = "Premium (Monthly)";

		public static readonly Guid PremiumMonthlyId = new Guid("6790a87c-86f9-40b8-9fc5-6228ae316d84");

		public const string PremiumAnnual = "Premium (Annual)";

		public static readonly Guid PremiumAnnualId = new Guid("4296c452-4654-4dd3-9ceb-16b228caa61f");
	}
}
