using Microsoft.Extensions.DependencyInjection;
using SkillStarLearning.SubscriptionRules.Application.Contracts.Persistence;
using SkillStarLearning.SubscriptionRules.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace SkillStarLearning.SubscriptionRules.Infrastructure
{
    public static class ApplicationServiceRegistration
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
        {
            services.AddSingleton<ISubscriptionRepository, InMemorySubscriptionRepository>();
            services.AddSingleton<IUserProfileRepository, InMemoryUserProfileRepository>();
            services.AddSingleton<IMembershipSignupRepository, InMemoryMembershipSignupRepository>();

            return services;
        }
    }
    
}
