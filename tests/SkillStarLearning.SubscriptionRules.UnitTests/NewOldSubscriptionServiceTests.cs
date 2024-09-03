using Microsoft.VisualStudio.TestTools.UnitTesting;
using SkillStarLearning.SubscriptionRules.Core.Enums;
using SkillStarLearning.SubscriptionRules.UnitTests.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace SkillStarLearning.SubscriptionRules.UnitTests
{
    [TestClass]
    public sealed class NewOldSubscriptionServiceTests
    {
        [TestMethod]
        public async Task SubscriptionService_ReturnsOnlineSubscriptionSettings_ForOverview()
        {
            var service = TestFactory.CreateOldSubscriptionService();

            var settings = await service.GetSubscriptionSettingsAsync("online-user-01");

            Assert.AreEqual(SubscriptionStatus.Active, settings.Status);
        }

        [TestMethod]
        public async Task SubscriptionService_ReturnsSimpleSubscriptionSummary_ForWidget()
        {
            var service = TestFactory.CreateSubscriptionService();

            var summary = await service.GetSubscriptionWidgetSummaryAsync("online-user-01");

            Assert.AreEqual(SubscriptionStatus.Active, summary.Status);
            Assert.AreEqual("Active subscription", summary.ShortDisplayLabel);
        }

        [TestMethod]
        public async Task OldSubscriptionService_ReturnsMembershipProfileFields_WhenMembershipCommunitySubscription()
        {
            var service = TestFactory.CreateOldSubscriptionService();

            var overview = await service.GetSubscriptionSettingsAsync("community-user-01");

            Assert.AreEqual(SubscriptionType.CommunityMembershipSubscription, overview.SubscriptionType);
            Assert.AreEqual("Copenhagen North", overview.Profile.LocalCommunityRegion);
            Assert.IsTrue(overview.Profile.AllowsEventCommunication);
            Assert.IsTrue(overview.Profile.HasAcceptedMembershipTerms);
            Assert.AreEqual("Step-free access requested", overview.Profile.AccessibilityNotes);
        }

        [TestMethod]
        public async Task OldSubscriptionService_CommunityMembershipSubscription_GetNudge_WhenIncompleteProfile()
        {
            var service = TestFactory.CreateOldSubscriptionService();

            var overview = await service.GetSubscriptionSettingsAsync("community-user-02");

            Assert.IsFalse(overview.Profile.AllowsEventCommunication);
            Assert.IsTrue(overview.RequiresMembershipProfileReview);
            
        }
    }

}
