using Microsoft.Extensions.DependencyInjection;
using SkillStarLearning.SubscriptionRules.Application.Contracts.Features.UpdateSubscriptionSettings;
using SkillStarLearning.SubscriptionRules.Application.Contracts.Services;
using SkillStarLearning.SubscriptionRules.Application.Services;

namespace SkillStarLearning.SubscriptionRules.Application
{
    public static class ApplicationServiceRegistration
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddSingleton(TimeProvider.System);
            services.AddScoped<IOldSubscriptionService, OldSubscriptionService>();
            services.AddScoped<INewSubscriptionService, NewSubscriptionService>();
            services.AddScoped<UpdateSubscriptionSettingsHandler, UpdateSubscriptionSettingsHandler>();
            services.AddScoped<IMembershipSignupService, MembershipSignupService>();
            services.AddScoped<ISubscriptionMessageService, SubscriptionMessageService>();
            services.AddScoped<IMarketSubscriptionPolicy, MarketSubscriptionPolicy>();
            services.AddScoped<ISubscriptionAvailabilityService, SubscriptionAvailabilityService>();

            return services;
        }
    }
}
