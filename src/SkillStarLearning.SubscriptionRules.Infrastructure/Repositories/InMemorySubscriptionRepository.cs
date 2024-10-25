using SkillStarLearning.SubscriptionRules.Application.Contracts.Persistence;
using SkillStarLearning.SubscriptionRules.Core.Entities;
using SkillStarLearning.SubscriptionRules.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SkillStarLearning.SubscriptionRules.Infrastructure.Repositories
{
    public sealed class InMemorySubscriptionRepository : ISubscriptionRepository
    {
        private readonly Dictionary<string, SubscriptionAccount> _fakeAccounts = new(StringComparer.OrdinalIgnoreCase)
        {
            ["online-user-01"] = new SubscriptionAccount
            {
                UserId = "online-user-01",
                SubscriptionType = SubscriptionType.OnlineSubscription,
                Status = SubscriptionStatus.Active,
                RenewalDate = DateTime.UtcNow.Date.AddDays(18),
                PaymentStatus = PaymentStatus.Paid,
                CreatedDate = DateTime.UtcNow.Date.AddDays(-30)
            },
            ["community-user-01"] = new SubscriptionAccount
            {
                UserId = "community-user-01",
                SubscriptionType = SubscriptionType.CommunityMembershipSubscription,
                Status = SubscriptionStatus.Active,
                RenewalDate = DateTime.UtcNow.Date.AddDays(22),
                PaymentStatus = PaymentStatus.Paid,
                CanManageSubscription = true,
                CreatedDate = DateTime.UtcNow.Date.AddDays(-3)
            },
            ["community-user-02"] = new SubscriptionAccount
            {
                UserId = "community-user-02",
                SubscriptionType = SubscriptionType.CommunityMembershipSubscription,
                Status = SubscriptionStatus.Active,
                RenewalDate = DateTime.UtcNow.Date.AddDays(22),
                PaymentStatus = PaymentStatus.Paid,
                CanManageSubscription = true,
                CreatedDate = DateTime.UtcNow.Date.AddDays(-3)
            },
            ["signup-user-01"] = new SubscriptionAccount
            {
                UserId = "signup-user-01",
                SubscriptionType = SubscriptionType.MembershipSignupSubscription,
                Status = SubscriptionStatus.Active,
                RenewalDate = DateTime.UtcNow.Date.AddDays(23),
                PaymentStatus = PaymentStatus.Paid,
                CanManageSubscription = true,
                CreatedDate = DateTime.UtcNow.Date.AddDays(-1)
            }
        };

        public Task<SubscriptionAccount?> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            _fakeAccounts.TryGetValue(userId, out var account);
            return Task.FromResult(account);
        }

        public Task SaveAsync(SubscriptionAccount account, CancellationToken cancellationToken = default)
        {
            _fakeAccounts[account.UserId] = account;
            return Task.CompletedTask;
        }
    }
}
