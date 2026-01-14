using Microsoft.VisualStudio.TestTools.UnitTesting;
using SkillStarLearning.SubscriptionRules.Application.Services;
using SkillStarLearning.SubscriptionRules.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SkillStarLearning.SubscriptionRules.UnitTests
{
    [TestClass]
    public sealed class SubscriptionMessageTests
    {
        [TestMethod]
        public void OnlineSubscription_FetchesDefaultMessage()
        {
            var service = new SubscriptionMessageService();

            var message = service.GetMessage(SubscriptionMessageFlowType.OnlineSubscription, Segmentation.SegmentationA);

            Assert.IsFalse(message.RefersToMembershipSignup);
        }

        [TestMethod]
        public void CommunityMembershipSubscription_FetchesDefaultMessage()
        {
            var service = new SubscriptionMessageService();

            var message = service.GetMessage(SubscriptionMessageFlowType.CommunityMembershipSubscription, Segmentation.SegmentationA);

            Assert.IsFalse(message.RefersToMembershipSignup);
        }

        [TestMethod]
        public void MembershipSignup_FetchesCustomCommunication()
        {
            var service = new SubscriptionMessageService();

            var message = service.GetMessage(SubscriptionMessageFlowType.MembershipSignup, Segmentation.SegmentationA);

            StringAssert.Contains(message.CustomerText, "Your community signup is complete");
            Assert.IsTrue(message.RefersToMembershipSignup);
        }

        [TestMethod]
        public void MarketB_SubscriptionMessageUsesLocalShorthandForCommunityMembership()
        {
            var service = new SubscriptionMessageService();

            var message = service.GetMessage(SubscriptionMessageFlowType.CommunityMembershipSubscription, Segmentation.SegmentationB);

            StringAssert.StartsWith(message.CustomerText, "Your community subscription is activated");
        }
    }

}
