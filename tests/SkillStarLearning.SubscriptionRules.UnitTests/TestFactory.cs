using SkillStarLearning.SubscriptionRules.Application.Contracts.Persistence;
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

        public static SubscriptionService CreateSubscriptionService()
        {
            return new SubscriptionService(CreateSubscriptionRepository(), CreateUserProfileRepository());
        }
    }

}
