using SkillStarLearning.SubscriptionRules.Application.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace SkillStarLearning.SubscriptionRules.Application.Contracts.Features.UpdateSubscriptionSettings
{
    public interface IUpdateSubscriptionSettingsHandler
    {
        Task<SubscriptionOverviewDto> Handle(
            UpdateSubscriptionSettingsCommand command,
            CancellationToken cancellationToken = default);
    }
}
