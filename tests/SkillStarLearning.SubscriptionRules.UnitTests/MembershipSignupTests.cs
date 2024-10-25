using Microsoft.VisualStudio.TestTools.UnitTesting;
using SkillStarLearning.SubscriptionRules.Application.Contracts.Features.CreateMembershipSignup;
using SkillStarLearning.SubscriptionRules.Application.Exceptions;
using SkillStarLearning.SubscriptionRules.Core.Enums;
using SkillStarLearning.SubscriptionRules.UnitTests.Util;
using System;
using System.Collections.Generic;
using System.Text;

namespace SkillStarLearning.SubscriptionRules.UnitTests
{
    [TestClass]
    public sealed class MembershipSignupTests
    {
        [TestMethod]
        public async Task SignupCreatesTrialSubscription_ButDoesNotCreatePaidSubscription_WhenNoConsent()
        {
            var now = new DateTimeOffset(2026, 5, 14, 10, 0, 0, TimeSpan.Zero);
            var service = TestFactory.CreateMembershipSignupService(new FixedTimeProvider(now));

            var result = await service.StartOfflineEventSignupAsync(new CreateMembershipSignupCommand
            {
                UserId = "event-attendee",
                StaffMember = "staff-1"
            });

            Assert.AreEqual(SubscriptionStatus.Trial, result.TrialStatus);
            Assert.AreEqual(now.UtcDateTime.AddDays(30), result.TrialEndsOn);
            Assert.IsTrue(result.CreatesPaidSubscription);
        }
    }

}
