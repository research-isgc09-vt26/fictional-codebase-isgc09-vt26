using SkillStarLearning.SubscriptionRules.Core.Common;
using SkillStarLearning.SubscriptionRules.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SkillStarLearning.SubscriptionRules.Core.Entities
{
    public sealed class MembershipSignup : AuditableEntity
    {
        public Guid SignupId { get; set; } = Guid.NewGuid();
        public string UserId { get; set; } = string.Empty;
        public MembershipSignupType SignupType { get; set; }
        public string StaffMember { get; set; } = string.Empty;
        public DateTime TrialStartsOn { get; set; }
        public DateTime TrialEndsOn { get; set; }
        public bool CreatesPaidSubscription { get; set; }
        public required Segmentation Segmentation { get; set; }
    }
}
