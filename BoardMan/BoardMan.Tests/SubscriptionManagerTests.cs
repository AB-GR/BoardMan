using BoardMan.Web.Data;
using BoardMan.Web.Managers;
using BoardMan.Web.Models;
using GenFu;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using MockQueryable.Moq;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace BoardMan.Tests
{
	public class SubscriptionManagerTests
	{
		private AppUser appUser = A.New<AppUser>();

		[Theory]
		[MemberData(nameof(Data))]
		public async Task GetSubscriptionNotificationAsync_Test(bool validUser, 
			bool requireSubscriptions, 
			DateTime? subscriptionEndDate, DateTime? planEndDate, SubscriptionStatus expected, bool isPlanIdNull)
		{
			// arrange
			var dbContext = InitializeData(requireSubscriptions, subscriptionEndDate, planEndDate);
			var inMemorySettings = new Dictionary<string, string> {
				{"SubscriptionAboutToExpireDays", "7"}				
			};

			IConfiguration configuration = new ConfigurationBuilder().AddInMemoryCollection(inMemorySettings).Build();

			var subManager = new SubscriptionManager(dbContext.Object, configuration);

			// act
			var subNotificationVm = await subManager.GetSubscriptionNotificationAsync(validUser ? appUser.Id : Guid.NewGuid());

			// assert
			Assert.Equal(expected, subNotificationVm.SubscriptionStatus);
			Assert.Equal(isPlanIdNull, subNotificationVm.PriorPlanId == null);
		}

		public static IEnumerable<object[]> Data =>
		new List<object[]>
		{
			new object[] { false, false, null, null, SubscriptionStatus.NoSubscriptionAvailable, true },
			new object[] { true, false, null, null, SubscriptionStatus.NoSubscriptionAvailable, true },
			new object[] { true, true, DateTime.UtcNow.AddDays(5), null, SubscriptionStatus.SubscriptionAboutToExpire, false },
			new object[] { true, true, DateTime.UtcNow.AddDays(8).AddMinutes(1), null, SubscriptionStatus.SubscriptionValid, false },
			new object[] { true, true, DateTime.UtcNow.AddDays(5), DateTime.UtcNow.AddDays(-1), SubscriptionStatus.SubscriptionAboutToExpire, true }
		};

		private Mock<BoardManDbContext> InitializeData(bool requireSubscriptions = true, DateTime? subscriptionEndDate = null, DateTime? planEndDate = null)
		{
			var context = new Mock<BoardManDbContext>();
			var planDiscounts = A.ListOf<DbPlanDiscount>(2);
			planDiscounts.ForEach(x =>
			{
				x.Plan = A.New<DbPlan>();
				x.PlanId = x.Plan.Id;				
				x.Plan.ExpireAt = planEndDate ?? DateTime.UtcNow.AddDays(1);
			});

			var plans = planDiscounts.Select(x => x.Plan).ToList();

			var subscriptions = new List<DbSubscription>();
			var paymentTransactions = new List<DbPaymentTrasaction>();

			if (requireSubscriptions)
			{
				subscriptions = A.ListOf<DbSubscription>(2);
				subscriptions.ForEach(x =>
				{
					var i = 0;
					x.Id = Guid.NewGuid();
					x.StartedAt = DateTime.UtcNow.AddDays(-10);
					x.ExpireAt = subscriptionEndDate ?? DateTime.UtcNow;
					x.UserId = appUser.Id;
					x.AppUser = appUser;
					x.DeletedAt = null;
					x.PaymentTrasaction = A.New<DbPaymentTrasaction>();
					x.PaymentTrasactionId = x.PaymentTrasaction.Id;
					x.PaymentTrasaction.PlanDiscount = planDiscounts[i];
					x.PaymentTrasaction.Plan = planDiscounts[i].Plan;
					i++;
				});

				paymentTransactions = subscriptions.Select(x => x.PaymentTrasaction).ToList();			
				
			}

			Mock<DbSet<DbSubscription>> subscriptionsDbSet = subscriptions.AsQueryable().BuildMockDbSet();
			Mock<DbSet<DbPaymentTrasaction>> paymentTranscationsDbSet = paymentTransactions.AsQueryable().BuildMockDbSet();
			Mock<DbSet<DbPlan>> plansDbSet = plans.AsQueryable().BuildMockDbSet();
			Mock<DbSet<DbPlanDiscount>> plansDiscountsDbSet = planDiscounts.AsQueryable().BuildMockDbSet();					
			context.Setup(c => c.Plans).Returns(plansDbSet.Object);
			context.Setup(c => c.PlanDiscounts).Returns(plansDiscountsDbSet.Object);
			context.Setup(c => c.Subscriptions).Returns(subscriptionsDbSet.Object);
			context.Setup(c => c.PaymentTransactions).Returns(paymentTranscationsDbSet.Object);

			return context;
		}
	}
}