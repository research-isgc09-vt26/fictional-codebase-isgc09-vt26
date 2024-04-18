using Microsoft.VisualStudio.TestTools.UnitTesting;
using SkillStarLearning.SubscriptionRules.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SkillStarLearning.SubscriptionRules.UnitTests
{
    [TestClass]
    public sealed class NewOldSubscriptionServiceTests
    {
        [TestMethod]
        public async Task SubscriptionService_ReturnsOnlineSubscriptionSettings()
        {
            var service = TestFactory.CreateOldSubscriptionService();

            var settings = await service.GetSubscriptionSettingsAsync("online-user-01");

            Assert.AreEqual(SubscriptionStatus.Active, settings.Status);
        }

        [TestMethod]
        public async Task SubscriptionService_ReturnsSimpleSubscriptionSummary_WhenOnlineSubscription()
        {
            var service = TestFactory.CreateSubscriptionService();

            var summary = await service.GetSubscriptionWidgetSummaryAsync("online-user-01");

            Assert.AreEqual(SubscriptionStatus.Active, summary.Status);
            Assert.AreEqual("Active subscription", summary.ShortDisplayLabel);
        }
    }

}
