using System;
using System.Collections.Generic;
using System.Text;

namespace SkillStarLearning.SubscriptionRules.Core.Entities
{
    public sealed class SubscriptionMessage
    {
        public string MessageCode { get; set; } = string.Empty;
        public string CustomerText { get; set; } = string.Empty;
        public bool IsForMembershipSignup { get; set; }
    }
}
