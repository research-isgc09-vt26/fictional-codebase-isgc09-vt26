using SkillStarLearning.SubscriptionRules.Core.Entities;

namespace SkillStarLearning.SubscriptionRules.Application.Contracts.Persistence
{
    public interface IUserProfileRepository
    {
        Task<UserProfile?> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);
        Task SaveAsync(UserProfile profile, CancellationToken cancellationToken = default);
    }
}
