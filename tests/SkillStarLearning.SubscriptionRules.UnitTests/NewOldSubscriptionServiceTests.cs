using Microsoft.VisualStudio.TestTools.UnitTesting;
using SkillStarLearning.SubscriptionRules.Application.Exceptions;
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

        [TestMethod]
        public async Task SubscriptionService_MembershipSignupSubscription_IncludesSignupInfo()
        {
            var service = TestFactory.CreateOldSubscriptionService();

            var overview = await service.GetSubscriptionSettingsAsync("signup-user-01");

            Assert.IsNotNull(overview.SignupInfo);
        }

        [TestMethod]
        public async Task SubscriptionService_ExtendedWidget_CommunityMembership_IncludesContactDetailsAndPreferences()
        {
            var service = TestFactory.CreateSubscriptionService();

            var widget = await service.GetExtendedSubscriptionWidgetAsync("community-user-01");

            Assert.AreEqual(SubscriptionType.CommunityMembershipSubscription, widget.SubscriptionType);
            Assert.AreEqual("john.doe@example.test", widget.Profile.Email);
            Assert.AreEqual("+45 55 66 77 88", widget.Profile.PhoneNumber);
            Assert.AreEqual("Copenhagen North", widget.Profile.LocalCommunityRegion);
            Assert.IsTrue(widget.Profile.AllowsEventCommunication);
            Assert.AreEqual("Step-free access requested", widget.Profile.AccessibilityNotes);
            Assert.IsFalse(widget.RequiresMembershipProfileReview);
        }

        [TestMethod]
        public async Task SubscriptionService_ExtendedWidget_CommunityMembership_IncludesNudge_WhenIncompleteProfile()
        {
            var service = TestFactory.CreateSubscriptionService();

            var widget = await service.GetExtendedSubscriptionWidgetAsync("community-user-02");

            Assert.IsFalse(widget.Profile.AllowsEventCommunication);
            Assert.IsTrue(widget.RequiresMembershipProfileReview);
        }

        [TestMethod]
        public async Task SubscriptionService_ExtendedWidget_NotAvailable_ForOnlineSubscription()
        {
            var service = TestFactory.CreateSubscriptionService();

            await Assert.ThrowsExceptionAsync<NotFoundException>(
                () => service.GetExtendedSubscriptionWidgetAsync("online-user-01"));
        }
    }

}
