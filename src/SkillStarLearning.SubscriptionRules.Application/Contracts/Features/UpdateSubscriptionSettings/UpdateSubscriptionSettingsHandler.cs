using SkillStarLearning.SubscriptionRules.Application.Contracts.Persistence;
using SkillStarLearning.SubscriptionRules.Application.Exceptions;
using SkillStarLearning.SubscriptionRules.Application.Models;
using SkillStarLearning.SubscriptionRules.Application.Services;
using SkillStarLearning.SubscriptionRules.Core.Entities;
using SkillStarLearning.SubscriptionRules.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SkillStarLearning.SubscriptionRules.Application.Contracts.Features.UpdateSubscriptionSettings
{
    public sealed class UpdateSubscriptionSettingsHandler : IUpdateSubscriptionSettingsHandler
    {
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly IUserProfileRepository _userProfileRepository;
        private readonly OldSubscriptionService _subscriptionService;
        private readonly IMembershipSignupRepository _membershipSignupRepository;

        public UpdateSubscriptionSettingsHandler(
            ISubscriptionRepository subscriptionRepository,
            IUserProfileRepository userProfileRepository,
            OldSubscriptionService subscriptionService,
            IMembershipSignupRepository membershipSignupRepository)
        {
            _subscriptionRepository = subscriptionRepository;
            _userProfileRepository = userProfileRepository;
            _subscriptionService = subscriptionService;
            _membershipSignupRepository = membershipSignupRepository;
        }

        public async Task<SubscriptionOverviewDto> Handle(
            UpdateSubscriptionSettingsCommand command,
            CancellationToken cancellationToken = default)
        {
            var account = await _subscriptionRepository.GetByUserIdAsync(command.UserId, cancellationToken)
                ?? throw new NotFoundException(nameof(SubscriptionAccount), command.UserId);

            var profile = await _userProfileRepository.GetByUserIdAsync(command.UserId, cancellationToken)
                ?? throw new NotFoundException(nameof(UserProfile), command.UserId);

            if (account.SubscriptionType == SubscriptionType.CommunityMembershipSubscription)
            {
                ValidateMembershipFields(command);
            }

            if (account.SubscriptionType == SubscriptionType.OnlineSubscription
                || account.SubscriptionType == SubscriptionType.CommunityMembershipSubscription)
            {
                ValidateSmsMarketingConsent(command);
            }

            profile.FullName = command.FullName;
            profile.PreferredDisplayName = command.PreferredDisplayName;
            profile.PhoneNumber = command.PhoneNumber;
            profile.BillingAddress = command.BillingAddress;
            profile.LocalCommunityRegion = command.LocalCommunityRegion;
            profile.AllowsEventCommunication = command.AllowsEventCommunication;
            profile.HasAcceptedMembershipTerms = command.HasAcceptedMembershipTerms;
            profile.AccessibilityNotes = command.AccessibilityNotes;
            profile.EmergencyContactPreference = command.EmergencyContactPreference;
            profile.AcceptsSmsMarketing = command.AcceptsSmsMarketing;
            profile.LastModifiedDate = DateTime.UtcNow;

            await _userProfileRepository.SaveAsync(profile, cancellationToken);

            var signup = await _membershipSignupRepository.GetByUserIdAsync(command.UserId, cancellationToken);

            return _subscriptionService.ToOverview(account, profile, signup);
        }

        private static void ValidateMembershipFields(UpdateSubscriptionSettingsCommand command)
        {
            if (string.IsNullOrWhiteSpace(command.LocalCommunityRegion))
            {
                throw new BusinessRuleException("Community membership subscriptions require a local community region.");
            }

            if (!command.HasAcceptedMembershipTerms)
            {
                throw new BusinessRuleException("Community membership terms must be accepted for offline membership administration.");
            }
        }

        private static void ValidateSmsMarketingConsent(UpdateSubscriptionSettingsCommand command)
        {
            if (command.AcceptsSmsMarketing && string.IsNullOrWhiteSpace(command.PhoneNumber))
            {
                throw new BusinessRuleException("A phone number is required when SMS marketing consent is selected.");
            }
        }
    }
}
