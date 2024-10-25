using SkillStarLearning.SubscriptionRules.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SkillStarLearning.SubscriptionRules.Application.Models
{
    public sealed class SubscriptionMessageDto
    {
        public SubscriptionMessageFlowType FlowType { get; set; }
        public string CustomerText { get; set; } = string.Empty;
        public bool RefersToMembershipSignup { get; set; }
    }
}
