using SkillStarLearning.SubscriptionRules.Core.Entities;

namespace SkillStarLearning.SubscriptionRules.Application.Contracts.Persistence
{
    public interface ISubscriptionRepository
    {
        Task<SubscriptionAccount?> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);
        Task SaveAsync(SubscriptionAccount account, CancellationToken cancellationToken = default);
    }
}
