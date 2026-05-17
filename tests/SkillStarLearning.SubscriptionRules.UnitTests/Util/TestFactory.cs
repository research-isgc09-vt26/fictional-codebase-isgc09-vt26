using SkillStarLearning.SubscriptionRules.Application.Contracts.Infrastructure;
using SkillStarLearning.SubscriptionRules.Application.Contracts.Persistence;
using SkillStarLearning.SubscriptionRules.Application.Contracts.Services;
using SkillStarLearning.SubscriptionRules.Application.Services;
using SkillStarLearning.SubscriptionRules.Infrastructure.AuditLogWriter;
using SkillStarLearning.SubscriptionRules.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace SkillStarLearning.SubscriptionRules.UnitTests.Util
{
    internal static class TestFactory
    {
        public static ISubscriptionRepository CreateSubscriptionRepository() => new InMemorySubscriptionRepository();

        public static IUserProfileRepository CreateUserProfileRepository() => new InMemoryUserProfileRepository();

        public static IMembershipSignupRepository CreateMembershipSignupRepository() => new InMemoryMembershipSignupRepository();

        public static IAuditLogWriter CreateAuditLogWriter() => new AuditLogWriter();

        public static IMarketSubscriptionPolicy CreateMarketSubscriptionPolicy() => new MarketSubscriptionPolicy();

        public static ISubscriptionMessageService CreateSubscriptionMessageService() => new SubscriptionMessageService();

        public static OldSubscriptionService CreateOldSubscriptionService()
        {
            return new  OldSubscriptionService(CreateSubscriptionRepository(), CreateUserProfileRepository(), CreateMembershipSignupRepository());
        }

        public static NewSubscriptionService CreateSubscriptionService()
        {
            // Share repositories between the old and new services so the widget reads
            // and the overview reads see the same fixtures.
            var subscriptionRepository = CreateSubscriptionRepository();
            var oldService = new OldSubscriptionService(
                subscriptionRepository,
                CreateUserProfileRepository(),
                CreateMembershipSignupRepository());
            return new NewSubscriptionService(subscriptionRepository, oldService);
        }

        public static MembershipSignupService CreateMembershipSignupService(TimeProvider? timeProvider = null)
        {
            return new MembershipSignupService(
                CreateMembershipSignupRepository(),
                CreateAuditLogWriter(),
                CreateMarketSubscriptionPolicy(),
                CreateSubscriptionMessageService(),
                timeProvider ?? TimeProvider.System);
        }
    }

}
