using SkillStarLearning.SubscriptionRules.Application.Contracts.Persistence;
using SkillStarLearning.SubscriptionRules.Application.Services;
using SkillStarLearning.SubscriptionRules.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace SkillStarLearning.SubscriptionRules.UnitTests
{
    internal static class TestFactory
    {
        public static ISubscriptionRepository CreateSubscriptionRepository() => new InMemorySubscriptionRepository();

        public static IUserProfileRepository CreateUserProfileRepository() => new InMemoryUserProfileRepository();

        public static OldSubscriptionService CreateOldSubscriptionService()
        {
            return new OldSubscriptionService(CreateSubscriptionRepository(), CreateUserProfileRepository());
        }

        public static NewSubscriptionService CreateSubscriptionService()
        {
            return new NewSubscriptionService(CreateSubscriptionRepository());
        }
    }

}
