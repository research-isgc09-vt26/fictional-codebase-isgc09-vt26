using System;
using System.Collections.Generic;
using System.Text;

namespace SkillStarLearning.SubscriptionRules.Application.Contracts.Infrastructure
{
    public interface IAuditLogWriter
    {
        Task WriteAsync(string message, CancellationToken cancellationToken = default);
    }
}
