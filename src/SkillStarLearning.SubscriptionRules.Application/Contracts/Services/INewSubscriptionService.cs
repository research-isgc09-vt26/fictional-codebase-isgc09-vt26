using SkillStarLearning.SubscriptionRules.Application.Models;
using SkillStarLearning.SubscriptionRules.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SkillStarLearning.SubscriptionRules.Application.Contracts.Services
{
    public interface INewSubscriptionService
    {
        Task<SimpleSubscriptionSummaryDto> GetSubscriptionWidgetSummaryAsync(
            string userId,
            CancellationToken cancellationToken = default);
    }
}
