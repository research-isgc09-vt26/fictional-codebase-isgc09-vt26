using SkillStarLearning.SubscriptionRules.Application.Contracts.Persistence;
using SkillStarLearning.SubscriptionRules.Application.Contracts.Services;
using SkillStarLearning.SubscriptionRules.Application.Exceptions;
using SkillStarLearning.SubscriptionRules.Application.Models;
using SkillStarLearning.SubscriptionRules.Core.Entities;
using SkillStarLearning.SubscriptionRules.Core.Enums;

namespace SkillStarLearning.SubscriptionRules.Application.Services
{
    // TODO: I am temporarily renaming it until we have time to move everything properly to NewSubscriptionService
    //       See docs/subscription-service-refactoring-notes.md for more details
    public sealed class OldSubscriptionService : IOldSubscriptionService
    {
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly IUserProfileRepository _userProfileRepository;

        public OldSubscriptionService(
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

        // Don't use it anymore! Refactor and move to NewSubscriptionService!
        // UPD: Maybe there is more point keeping it - waiting clarification from tech lead and legal
        public SubscriptionOverviewDto ToOverview(SubscriptionAccount account, UserProfile profile)
        {
            var nudgeToReviewProfile = account.SubscriptionType == SubscriptionType.CommunityMembershipSubscription
                && (!profile.AllowsEventCommunication || string.IsNullOrEmpty(profile.PhoneNumber));
            
            return new SubscriptionOverviewDto
            {
                UserId = account.UserId,
                SubscriptionType = account.SubscriptionType,
                Status = account.Status,
                RenewalDate = account.RenewalDate,
                PaymentStatus = account.PaymentStatus,
                CanManageSubscription = account.CanManageSubscription,
                RequiresMembershipProfileReview = nudgeToReviewProfile,
                Profile = new SubscriptionProfileDto
                {
                    UserId = profile.UserId,
                    PreferredDisplayName = profile.PreferredDisplayName,
                    Email = profile.Email,
                    BillingAddress = profile.BillingAddress,
                    HasAcceptedMembershipTerms = profile.HasAcceptedMembershipTerms,
                    FullName = profile.FullName,
                    PhoneNumber = profile.PhoneNumber,
                    LocalCommunityRegion = profile.LocalCommunityRegion,
                    AllowsEventCommunication = profile.AllowsEventCommunication,
                    AccessibilityNotes = profile.AccessibilityNotes,
                    EmergencyContactPreference = profile.EmergencyContactPreference
                }
            };
        }
    }
}
