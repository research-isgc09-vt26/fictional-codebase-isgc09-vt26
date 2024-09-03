using SkillStarLearning.SubscriptionRules.Application.Contracts.Infrastructure;
using System;
using System.Collections.Generic;
using System.Text;

namespace SkillStarLearning.SubscriptionRules.Infrastructure.AuditLogWriter
{
    public sealed class AuditLogWriter : IAuditLogWriter
    {
        private readonly List<string> _entries = [];

        public IReadOnlyList<string> Entries => _entries;

        public Task WriteAsync(string message, CancellationToken cancellationToken = default)
        {
            _entries.Add($"{DateTime.UtcNow:o} {message}");
            return Task.CompletedTask;
        }
    }
}
