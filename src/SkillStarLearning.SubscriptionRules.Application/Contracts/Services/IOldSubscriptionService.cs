using SkillStarLearning.SubscriptionRules.Application.Models;
using SkillStarLearning.SubscriptionRules.Core.Entities;

namespace SkillStarLearning.SubscriptionRules.Application.Contracts.Services
{
    public interface IOldSubscriptionService
    {
        Task<SubscriptionOverviewDto> GetSubscriptionSettingsAsync(
            string userId,
            CancellationToken cancellationToken = default);

        SubscriptionOverviewDto ToOverview(SubscriptionAccount account, UserProfile profile);
    }
}
