using SkillStarLearning.SubscriptionRules.Application.Contracts.Persistence;
using SkillStarLearning.SubscriptionRules.Application.Contracts.Services;
using SkillStarLearning.SubscriptionRules.Application.Exceptions;
using SkillStarLearning.SubscriptionRules.Application.Models;
using SkillStarLearning.SubscriptionRules.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SkillStarLearning.SubscriptionRules.Application.Services
{
    // TODO: Refactoring will be done in TECHDEBT-123 task. Currently used for the widget.
    public sealed class NewSubscriptionService : INewSubscriptionService
    {
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly IOldSubscriptionService _oldSubscriptionService;

        public NewSubscriptionService(
            ISubscriptionRepository subscriptionRepository,
            IOldSubscriptionService oldSubscriptionService)
        {
            _subscriptionRepository = subscriptionRepository;
            _oldSubscriptionService = oldSubscriptionService;
        }

        public async Task<SimpleSubscriptionSummaryDto> GetSubscriptionWidgetSummaryAsync(
            string userId,
            CancellationToken cancellationToken = default)
        {
            var account = await _subscriptionRepository.GetByUserIdAsync(userId, cancellationToken);

            var hasSubscription = account is not null &&
                (account.Status == SubscriptionStatus.Active ||
                    account.Status == SubscriptionStatus.Trial);

            if (!hasSubscription)
            {
                return new SimpleSubscriptionSummaryDto
                {
                    Status = account.Status,
                    ShortDisplayLabel = "No active subscription"
                };
            }

            return new SimpleSubscriptionSummaryDto
            {
                Status = account.Status,
                NextRenewalDate = account.RenewalDate,
                ShortDisplayLabel = "Active subscription"
            };
        }

        public async Task<SubscriptionOverviewDto> GetExtendedSubscriptionWidgetAsync(
            string userId,
            CancellationToken cancellationToken = default)
        {
            // The extended widget shows the same contact details, preferences, and profile-review
            // nudge as the ordinary overview, so we reuse OldSubscriptionService (per
            // docs/refactoring-notes.md, until TECHDEBT-123 moves the logic over).
            var overview = await _oldSubscriptionService.GetSubscriptionSettingsAsync(userId, cancellationToken);

            if (overview.SubscriptionType != SubscriptionType.CommunityMembershipSubscription)
            {
                throw new NotFoundException("ExtendedSubscriptionWidget", userId);
            }

            return overview;
        }
    }
}
