using Microsoft.VisualStudio.TestTools.UnitTesting;
using SkillStarLearning.SubscriptionRules.Application.Services;
using SkillStarLearning.SubscriptionRules.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SkillStarLearning.SubscriptionRules.UnitTests
{
    [TestClass]
    public sealed class MarketSubscriptionPolicyTests
    {
        [TestMethod]
        public void SegmentationA_AllowsOnlineSubscription()
        {
            var policy = new MarketSubscriptionPolicy();

            Assert.IsTrue(policy.IsSubscriptionAvailable(Segmentation.SegmentationA, SubscriptionType.OnlineSubscription));
        }

        [TestMethod]
        public void SegmentationA_AllowsMembershipSignup()
        {
            var policy = new MarketSubscriptionPolicy();

            Assert.IsTrue(policy.IsMembershipSignupAvailable(Segmentation.SegmentationA));
        }

        [TestMethod]
        public void SegmentationB_DoesNotAllowOnlineSubscription()
        {
            var policy = new MarketSubscriptionPolicy();

            Assert.IsFalse(policy.IsSubscriptionAvailable(Segmentation.SegmentationB, SubscriptionType.OnlineSubscription));
        }

        [TestMethod]
        public void SegmentationB_CommunityMemebrshipSubscription()
        {
            var policy = new MarketSubscriptionPolicy();

            Assert.IsTrue(policy.IsSubscriptionAvailable(Segmentation.SegmentationB, SubscriptionType.CommunityMembershipSubscription));
        }

        [TestMethod]
        public void SegmentationB_AllowsMembershipSignup()
        {
            var policy = new MarketSubscriptionPolicy();

            Assert.IsTrue(policy.IsMembershipSignupAvailable(Segmentation.SegmentationB));
        }

        [TestMethod]
        public void SegmentationB_DoesNotIncludeStandardFreeTrial()
        {
            var policy = new MarketSubscriptionPolicy();

            Assert.IsFalse(policy.IncludesStandardFreeTrial(Segmentation.SegmentationB));
        }

        [TestMethod]
        public void SegmentationA_IncludesStandardFreeTrial()
        {
            var policy = new MarketSubscriptionPolicy();

            Assert.IsTrue(policy.IncludesStandardFreeTrial(Segmentation.SegmentationA));
        }
    }

}
