using SkillStarLearning.SubscriptionRules.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SkillStarLearning.SubscriptionRules.Application.Contracts.Persistence
{
    public interface IMembershipSignupRepository
    {
        Task AddAsync(MembershipSignup signup, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<MembershipSignup>> ListAsync(CancellationToken cancellationToken = default);
        Task<MembershipSignup?> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default);

    }
}
