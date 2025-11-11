using SkillStarLearning.SubscriptionRules.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SkillStarLearning.SubscriptionRules.Application.Models
{
    public sealed class MembershipSignupResultDto
    {
        public Guid SignupId { get; set; }
        public required string UserId { get; set; }
        public MembershipSignupType SignupType { get; set; }
        public SubscriptionStatus TrialStatus { get; set; }
        public DateTime TrialStartsOn { get; set; }
        public DateTime TrialEndsOn { get; set; }
        public bool CreatesPaidSubscription { get; set; }
        public string CustomerMessage { get; set; } = string.Empty;
    }
}
