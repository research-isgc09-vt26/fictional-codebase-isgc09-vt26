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

            var message = service.GetMessage(SubscriptionMessageFlowType.OnlineSubscription);

            Assert.IsFalse(message.RefersToMembershipSignup);
        }

        [TestMethod]
        public void CommunityMembershipSubscription_FetchesDefaultMessage()
        {
            var service = new SubscriptionMessageService();

            var message = service.GetMessage(SubscriptionMessageFlowType.CommunityMembershipSubscription);

            Assert.IsFalse(message.RefersToMembershipSignup);
        }

        [TestMethod]
        public void MembershipSignup_FetchesCustomCommunication()
        {
            var service = new SubscriptionMessageService();

            var message = service.GetMessage(SubscriptionMessageFlowType.MembershipSignup);

            StringAssert.Contains(message.CustomerText, "Your community signup is complete");
            Assert.IsTrue(message.RefersToMembershipSignup);
        }
    }

}
