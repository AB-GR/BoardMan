using BoardMan.Web.Data;
using BoardMan.Web.Managers;
using BoardMan.Web.Models;
using GenFu;
using Microsoft.EntityFrameworkCore;
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
		private List<AppUser> appUsers = A.ListOf<AppUser>(2);

		[Fact]
		public async Task GetSubscriptionNotificationAsync_Test()
		{
			// arrange
			var context = CreateDbContext();

			var subManager = new SubscriptionManager(context.Object);

			// act
			var subscriptionVm = await subManager.GetSubscriptionNotificationAsync(appUsers[0].Id);

			// assert
			Assert.Equal(SubscriptionStatus.NoSubscriptionAvailable, subscriptionVm.SubscriptionStatus);
		}

		private Mock<BoardManDbContext> CreateDbContext()
		{
			var planDiscounts = A.ListOf<DbPlanDiscount>(2);
			planDiscounts.ForEach(x =>
			{
				x.Plan = A.New<DbPlan>();
				x.PlanId = x.Plan.Id;
			});

			var plans = planDiscounts.Select(x => x.Plan).ToList();

			var subscriptions = A.ListOf<DbSubscription>(2);
			subscriptions.ForEach(x =>
			{
				var i = 0;
				x.Id = Guid.NewGuid();
				x.UserId = appUsers[i].Id;
				x.AppUser = appUsers[i];
				x.PaymentTrasaction = A.New<DbPaymentTrasaction>();
				x.PaymentTrasactionId = x.PaymentTrasaction.Id;
				x.PaymentTrasaction.PlanDiscount = planDiscounts[i];
				x.PaymentTrasaction.Plan = planDiscounts[i].Plan;
				i++;
			});

			var paymentTransactions = subscriptions.Select(x => x.PaymentTrasaction).ToList();
			Mock<DbSet<DbSubscription>> subscriptionsDbSet = subscriptions.AsQueryable().BuildMockDbSet();
			Mock<DbSet<DbPlan>> plansDbSet = plans.AsQueryable().BuildMockDbSet();
			Mock<DbSet<DbPlanDiscount>> plansDiscountsDbSet = planDiscounts.AsQueryable().BuildMockDbSet();
			Mock<DbSet<DbPaymentTrasaction>> paymentTranscationsDbSet = paymentTransactions.AsQueryable().BuildMockDbSet();

			var context = new Mock<BoardManDbContext>();
			context.Setup(c => c.Subscriptions).Returns(subscriptionsDbSet.Object);
			context.Setup(c => c.Plans).Returns(plansDbSet.Object);
			context.Setup(c => c.PlanDiscounts).Returns(plansDiscountsDbSet.Object);
			context.Setup(c => c.PaymentTransactions).Returns(paymentTranscationsDbSet.Object);

			return context;
		}

		private static Mock<DbSet<T>> GenerateDbSet<T>(List<T> entityList) where T : class
		{
			var subscriptionsDbSet = new Mock<DbSet<T>>();			
			subscriptionsDbSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(entityList.AsQueryable().Provider);
			subscriptionsDbSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(entityList.AsQueryable().Expression);
			subscriptionsDbSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(entityList.AsQueryable().ElementType);
			subscriptionsDbSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(entityList.AsQueryable().GetEnumerator());
			return subscriptionsDbSet;
		}
	}
}