using SkillStarLearning.SubscriptionRules.Application.Contracts.Persistence;
using SkillStarLearning.SubscriptionRules.Application.Contracts.Services;
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

        public NewSubscriptionService(ISubscriptionRepository subscriptionRepository)
        {
            _subscriptionRepository = subscriptionRepository;
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
    }
}
