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

            Assert.AreEqual(SubscriptionMessageFlowType.OnlineSubscription, message.FlowType);
            Assert.AreEqual(string.Empty, message.CustomerText);
            Assert.IsFalse(message.RefersToMembershipSignup);
        }

        [TestMethod]
        public void CommunityMembershipSubscription_FetchesDefaultMessage()
        {
            var service = new SubscriptionMessageService();

            var message = service.GetMessage(SubscriptionMessageFlowType.CommunityMembershipSubscription, Segmentation.SegmentationA);

            Assert.AreEqual(SubscriptionMessageFlowType.CommunityMembershipSubscription, message.FlowType);
            Assert.AreEqual(string.Empty, message.CustomerText);
            Assert.IsFalse(message.RefersToMembershipSignup);
        }

        [TestMethod]
        public void MembershipSignup_FetchesCustomCommunication()
        {
            var service = new SubscriptionMessageService();

            var message = service.GetMessage(SubscriptionMessageFlowType.MembershipSignup, Segmentation.SegmentationA);

            Assert.AreEqual(SubscriptionMessageFlowType.MembershipSignup, message.FlowType);
            StringAssert.Contains(message.CustomerText, "Your community signup is complete");
            Assert.IsTrue(message.RefersToMembershipSignup);
        }

        [TestMethod]
        public void MarketB_SubscriptionMessageUsesLocalShorthandForCommunityMembership()
        {
            var service = new SubscriptionMessageService();

            var message = service.GetMessage(SubscriptionMessageFlowType.CommunityMembershipSubscription, Segmentation.SegmentationB);

            Assert.AreEqual(SubscriptionMessageFlowType.CommunityMembershipSubscription, message.FlowType);
            StringAssert.StartsWith(message.CustomerText, "Your community subscription is activated");
            Assert.IsFalse(message.RefersToMembershipSignup);
        }

        [TestMethod]
        public void MarketB_OnlineSubscription_UsesLocalShorthandRegardlessOfFlow()
        {
            var service = new SubscriptionMessageService();

            var message = service.GetMessage(SubscriptionMessageFlowType.OnlineSubscription, Segmentation.SegmentationB);

            Assert.AreEqual(SubscriptionMessageFlowType.OnlineSubscription, message.FlowType);
            StringAssert.StartsWith(message.CustomerText, "Your community subscription is activated");
            Assert.IsFalse(message.RefersToMembershipSignup);
        }

        [TestMethod]
        public void MarketB_MembershipSignup_UsesLocalShorthandAndDoesNotReferToSignup()
        {
            var service = new SubscriptionMessageService();

            var message = service.GetMessage(SubscriptionMessageFlowType.MembershipSignup, Segmentation.SegmentationB);

            Assert.AreEqual(SubscriptionMessageFlowType.MembershipSignup, message.FlowType);
            StringAssert.StartsWith(message.CustomerText, "Your community subscription is activated");
            Assert.IsFalse(message.RefersToMembershipSignup);
        }

        [TestMethod]
        public void UnsupportedFlowType_ThrowsArgumentOutOfRangeException()
        {
            var service = new SubscriptionMessageService();

            Assert.ThrowsException<ArgumentOutOfRangeException>(() =>
                service.GetMessage((SubscriptionMessageFlowType)999, Segmentation.SegmentationA));
        }
    }

}
