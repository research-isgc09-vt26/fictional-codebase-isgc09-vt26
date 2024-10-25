using SkillStarLearning.SubscriptionRules.Application.Contracts.Infrastructure;
using SkillStarLearning.SubscriptionRules.Application.Contracts.Persistence;
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



        public static OldSubscriptionService CreateOldSubscriptionService()
        {
            return new  OldSubscriptionService(CreateSubscriptionRepository(), CreateUserProfileRepository(), CreateMembershipSignupRepository());
        }

        public static NewSubscriptionService CreateSubscriptionService()
        {
            return new NewSubscriptionService(CreateSubscriptionRepository());
        }

        public static MembershipSignupService CreateMembershipSignupService(TimeProvider? timeProvider = null)
        {
            return new MembershipSignupService(
                CreateMembershipSignupRepository(),
                CreateAuditLogWriter(),
                timeProvider ?? TimeProvider.System);
        }
    }

}
