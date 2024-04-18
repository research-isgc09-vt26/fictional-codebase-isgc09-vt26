using System;
using System.Collections.Generic;
using System.Text;

namespace SkillStarLearning.SubscriptionRules.Application.Exceptions
{
    public sealed class NotFoundException : Exception
    {
        public NotFoundException(string name, string key)
            : base($"{name} ({key}) was not found.")
        {
        }
    }
}
