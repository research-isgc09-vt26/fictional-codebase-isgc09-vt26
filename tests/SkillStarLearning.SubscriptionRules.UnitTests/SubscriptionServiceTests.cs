using Microsoft.VisualStudio.TestTools.UnitTesting;
using SkillStarLearning.SubscriptionRules.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SkillStarLearning.SubscriptionRules.UnitTests
{
    [TestClass]
    public sealed class SubscriptionServiceTests
    {
        [TestMethod]
        public async Task SubscriptionService_ReturnsOnlineSubscriptionSettings()
        {
            var service = TestFactory.CreateNewSubscriptionService();

            var settings = await service.GetSubscriptionSettingsAsync("online-user-01");

            Assert.AreEqual(SubscriptionStatus.Active, settings.Status);
        }
    }

}
