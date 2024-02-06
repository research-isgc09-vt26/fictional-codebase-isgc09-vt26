using SkillStarLearning.SubscriptionRules.Application.Contracts.Persistence;
using SkillStarLearning.SubscriptionRules.Application.Contracts.Services;
using SkillStarLearning.SubscriptionRules.Application.Models;
using SkillStarLearning.SubscriptionRules.Core.Entities;
using SkillStarLearning.SubscriptionRules.Core.Enums;

namespace SkillStarLearning.SubscriptionRules.Application.Services
{
    public sealed class SubscriptionService : ISubscriptionService
    {
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly IUserProfileRepository _userProfileRepository;

        public SubscriptionService(
            ISubscriptionRepository subscriptionRepository,
            IUserProfileRepository userProfileRepository)
        {
            _subscriptionRepository = subscriptionRepository;
            _userProfileRepository = userProfileRepository;
        }

        public async Task<SubscriptionOverviewDto> GetSubscriptionSettingsAsync(
            string userId,
            CancellationToken cancellationToken = default)
        {
            var account = await _subscriptionRepository.GetByUserIdAsync(userId, cancellationToken)
                ?? throw new NotFoundException(nameof(SubscriptionAccount), userId);

            var profile = await _userProfileRepository.GetByUserIdAsync(userId, cancellationToken)
                ?? throw new NotFoundException(nameof(UserProfile), userId);

            return ToOverview(account, profile);
        }

        public static SubscriptionOverviewDto ToOverview(SubscriptionAccount account, UserProfile profile)
        {
            return new SubscriptionOverviewDto
            {
                UserId = account.UserId,
                SubscriptionType = account.SubscriptionType,
                Status = account.Status,
                RenewalDate = account.RenewalDate,
                PaymentStatus = account.PaymentStatus,
                Profile = new SubscriptionProfileDto
                {
                    UserId = profile.UserId,
                    PreferredDisplayName = profile.PreferredDisplayName,
                    Email = profile.Email,
                    BillingAddress = profile.BillingAddress,
                    HasAcceptedMembershipTerms = profile.HasAcceptedMembershipTerms
                }
            };
        }
    }
}
