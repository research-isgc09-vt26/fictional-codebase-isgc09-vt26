using Microsoft.VisualStudio.TestTools.UnitTesting;
using SkillStarLearning.SubscriptionRules.Application.Contracts.Features.UpdateSubscriptionSettings;
using SkillStarLearning.SubscriptionRules.Application.Exceptions;
using SkillStarLearning.SubscriptionRules.UnitTests.Util;

namespace SkillStarLearning.SubscriptionRules.UnitTests
{
    [TestClass]
    public sealed class UpdateSubscriptionSettingsTests
    {
        [TestMethod]
        public async Task OnlineSubscription_AllowsSmsMarketingWithPhoneNumber()
        {
            var handler = TestFactory.CreateUpdateSubscriptionSettingsHandler();

            var overview = await handler.Handle(new UpdateSubscriptionSettingsCommand
            {
                UserId = "online-user-01",
                BillingAddress = "Online Street 10, 1000 Copenhagen",
                PhoneNumber = "+45 12 34 56 78",
                AllowsSmsMarketing = true
            });

            Assert.IsTrue(overview.Profile.AllowsSmsMarketing);
            Assert.AreEqual("+45 12 34 56 78", overview.Profile.PhoneNumber);
        }

        [TestMethod]
        public async Task OnlineSubscription_RejectsSmsMarketingWithoutPhoneNumber()
        {
            var handler = TestFactory.CreateUpdateSubscriptionSettingsHandler();

            await Assert.ThrowsExceptionAsync<BusinessRuleException>(() =>
                handler.Handle(new UpdateSubscriptionSettingsCommand
                {
                    UserId = "online-user-01",
                    BillingAddress = "Online Street 10, 1000 Copenhagen",
                    PhoneNumber = string.Empty,
                    AllowsSmsMarketing = true
                }));
        }

        [TestMethod]
        public async Task CommunityMembershipSubscription_RejectsSmsMarketingWithoutPhoneNumber()
        {
            var handler = TestFactory.CreateUpdateSubscriptionSettingsHandler();

            await Assert.ThrowsExceptionAsync<BusinessRuleException>(() =>
                handler.Handle(new UpdateSubscriptionSettingsCommand
                {
                    UserId = "community-user-01",
                    BillingAddress = "Member Road 42, 2200 Copenhagen",
                    LocalCommunityRegion = "Copenhagen North",
                    HasAcceptedMembershipTerms = true,
                    PhoneNumber = "   ",
                    AllowsSmsMarketing = true
                }));
        }

        [TestMethod]
        public async Task OnlineSubscription_PermitsEmptyPhoneNumberWhenSmsConsentNotGiven()
        {
            var handler = TestFactory.CreateUpdateSubscriptionSettingsHandler();

            var overview = await handler.Handle(new UpdateSubscriptionSettingsCommand
            {
                UserId = "online-user-01",
                BillingAddress = "Online Street 10, 1000 Copenhagen",
                PhoneNumber = string.Empty,
                AllowsSmsMarketing = false
            });

            Assert.IsFalse(overview.Profile.AllowsSmsMarketing);
        }
    }
}
