using SkillStarLearning.SubscriptionRules.Application.Contracts.Persistence;
using SkillStarLearning.SubscriptionRules.Core.Entities;
using SkillStarLearning.SubscriptionRules.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SkillStarLearning.SubscriptionRules.Infrastructure.Repositories
{
    public sealed class InMemoryMembershipSignupRepository : IMembershipSignupRepository
    {
        private readonly List<MembershipSignup> _fakeSignups = [
            new MembershipSignup
            {
                UserId = "signup-user-01",
                SignupType = MembershipSignupType.OfflineSignup,
                StaffMember = "Karl Karlsson",
                CreatedDate = DateTime.UtcNow.Date.AddDays(-2),
                TrialStartsOn  = DateTime.UtcNow.Date.AddDays(-2),
                TrialEndsOn = DateTime.UtcNow.Date.AddDays(28),
                CreatesPaidSubscription = false
            },
        ];

        public Task AddAsync(MembershipSignup signup, CancellationToken cancellationToken = default)
        {
            _fakeSignups.Add(signup);
            return Task.CompletedTask;
        }

        public Task<IReadOnlyList<MembershipSignup>> ListAsync(CancellationToken cancellationToken = default)
        {
            return Task.FromResult<IReadOnlyList<MembershipSignup>>(_fakeSignups);
        }

        public Task<MembershipSignup?> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            var signup = _fakeSignups.FirstOrDefault(x => x.UserId == userId);
            return Task.FromResult(signup);
        }
    }
}
