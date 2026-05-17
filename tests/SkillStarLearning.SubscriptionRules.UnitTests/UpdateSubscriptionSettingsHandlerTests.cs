using Microsoft.VisualStudio.TestTools.UnitTesting;
using SkillStarLearning.SubscriptionRules.Application.Contracts.Features.UpdateSubscriptionSettings;
using SkillStarLearning.SubscriptionRules.Application.Exceptions;
using SkillStarLearning.SubscriptionRules.UnitTests.Util;

namespace SkillStarLearning.SubscriptionRules.UnitTests
{
    [TestClass]
    public sealed class UpdateSubscriptionSettingsHandlerTests
    {
        [TestMethod]
        public async Task OnlineSubscription_SmsConsentWithPhoneNumber_PersistsConsent()
        {
            var handler = TestFactory.CreateUpdateSubscriptionSettingsHandler();

            var overview = await handler.Handle(new UpdateSubscriptionSettingsCommand
            {
                UserId = "online-user-01",
                BillingAddress = "Online Street 10, 1000 Copenhagen",
                PhoneNumber = "+45 12 34 56 78",
                AcceptsSmsMarketing = true
            });

            Assert.IsTrue(overview.Profile.AcceptsSmsMarketing);
            Assert.AreEqual("+45 12 34 56 78", overview.Profile.PhoneNumber);
        }

        [TestMethod]
        public async Task OnlineSubscription_SmsConsentWithoutPhoneNumber_IsRejected()
        {
            var handler = TestFactory.CreateUpdateSubscriptionSettingsHandler();

            await Assert.ThrowsExceptionAsync<BusinessRuleException>(() =>
                handler.Handle(new UpdateSubscriptionSettingsCommand
                {
                    UserId = "online-user-01",
                    BillingAddress = "Online Street 10, 1000 Copenhagen",
                    PhoneNumber = string.Empty,
                    AcceptsSmsMarketing = true
                }));
        }

        [TestMethod]
        public async Task CommunityMembershipSubscription_SmsConsentWithPhoneNumber_PersistsConsent()
        {
            var handler = TestFactory.CreateUpdateSubscriptionSettingsHandler();

            var overview = await handler.Handle(new UpdateSubscriptionSettingsCommand
            {
                UserId = "community-user-01",
                BillingAddress = "Member Road 42, 2200 Copenhagen",
                PhoneNumber = "+45 55 66 77 88",
                LocalCommunityRegion = "Copenhagen North",
                HasAcceptedMembershipTerms = true,
                AllowsEventCommunication = true,
                AcceptsSmsMarketing = true
            });

            Assert.IsTrue(overview.Profile.AcceptsSmsMarketing);
        }

        [TestMethod]
        public async Task CommunityMembershipSubscription_SmsConsentWithoutPhoneNumber_IsRejected()
        {
            var handler = TestFactory.CreateUpdateSubscriptionSettingsHandler();

            await Assert.ThrowsExceptionAsync<BusinessRuleException>(() =>
                handler.Handle(new UpdateSubscriptionSettingsCommand
                {
                    UserId = "community-user-01",
                    BillingAddress = "Member Road 42, 2200 Copenhagen",
                    PhoneNumber = "   ",
                    LocalCommunityRegion = "Copenhagen North",
                    HasAcceptedMembershipTerms = true,
                    AllowsEventCommunication = true,
                    AcceptsSmsMarketing = true
                }));
        }

        [TestMethod]
        public async Task OnlineSubscription_NoSmsConsent_NoPhoneNumberRequired()
        {
            var handler = TestFactory.CreateUpdateSubscriptionSettingsHandler();

            var overview = await handler.Handle(new UpdateSubscriptionSettingsCommand
            {
                UserId = "online-user-01",
                BillingAddress = "Online Street 10, 1000 Copenhagen",
                PhoneNumber = string.Empty,
                AcceptsSmsMarketing = false
            });

            Assert.IsFalse(overview.Profile.AcceptsSmsMarketing);
        }
    }
}
