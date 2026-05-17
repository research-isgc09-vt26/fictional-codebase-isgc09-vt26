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
    // TODO: Refactoring is planned in TECHDEBT-123
    public sealed class OldSubscriptionService : IOldSubscriptionService
    {
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly IUserProfileRepository _userProfileRepository;
        private readonly IMembershipSignupRepository _membershipSignupRepository;

        public OldSubscriptionService(
            ISubscriptionRepository subscriptionRepository,
            IUserProfileRepository userProfileRepository,
            IMembershipSignupRepository membershipSignupRepository)
        {
            _subscriptionRepository = subscriptionRepository;
            _userProfileRepository = userProfileRepository;
            _membershipSignupRepository = membershipSignupRepository;
        }

        public async Task<SubscriptionOverviewDto> GetSubscriptionSettingsAsync(
            string userId,
            CancellationToken cancellationToken = default)
        {
            var account = await _subscriptionRepository.GetByUserIdAsync(userId, cancellationToken)
                ?? throw new NotFoundException(nameof(SubscriptionAccount), userId);

            var profile = await _userProfileRepository.GetByUserIdAsync(userId, cancellationToken)
                ?? throw new NotFoundException(nameof(UserProfile), userId);

            var signupInfo = await _membershipSignupRepository.GetByUserIdAsync(userId, cancellationToken);

            return ToOverview(account, profile, signupInfo);
        }

        public async Task<SubscriptionOverviewDto?> GetExtendedSubscriptionWidgetAsync(
            string userId,
            CancellationToken cancellationToken = default)
        {
            var account = await _subscriptionRepository.GetByUserIdAsync(userId, cancellationToken);
            if (account is null || account.SubscriptionType != SubscriptionType.CommunityMembershipSubscription)
            {
                return null;
            }

            var profile = await _userProfileRepository.GetByUserIdAsync(userId, cancellationToken)
                ?? throw new NotFoundException(nameof(UserProfile), userId);

            var signupInfo = await _membershipSignupRepository.GetByUserIdAsync(userId, cancellationToken);

            return ToOverview(account, profile, signupInfo);
        }

        public SubscriptionOverviewDto ToOverview(SubscriptionAccount account, UserProfile profile, MembershipSignup? signupInfo)
        {
            var nudgeToReviewProfile = (signupInfo is not null && signupInfo.Segmentation != Segmentation.SegmentationB) 
                || (account.SubscriptionType == SubscriptionType.CommunityMembershipSubscription
                && (!profile.AllowsEventCommunication || string.IsNullOrEmpty(profile.PhoneNumber)));
            
            return new SubscriptionOverviewDto
            {
                UserId = account.UserId,
                SubscriptionType = account.SubscriptionType,
                Status = account.Status,
                RenewalDate = account.RenewalDate,
                PaymentStatus = account.PaymentStatus,
                CanManageSubscription = account.CanManageSubscription,
                RequiresMembershipProfileReview = nudgeToReviewProfile,
                SignupInfo = signupInfo is null ? null : new MembershipSignupResultDto
                {
                    UserId = signupInfo.UserId,
                    SignupType = signupInfo.SignupType,
                    TrialStartsOn = signupInfo.TrialStartsOn,
                    TrialEndsOn = signupInfo.TrialEndsOn,
                    CreatesPaidSubscription = signupInfo.CreatesPaidSubscription
                },
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
