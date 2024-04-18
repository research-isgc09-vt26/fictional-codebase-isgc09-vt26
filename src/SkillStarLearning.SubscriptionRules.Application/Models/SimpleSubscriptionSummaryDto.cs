using SkillStarLearning.SubscriptionRules.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SkillStarLearning.SubscriptionRules.Application.Models
{
    public sealed class SimpleSubscriptionSummaryDto
    {
        public SubscriptionStatus Status { get; set; }
        public DateTime? NextRenewalDate { get; set; }
        public string ShortDisplayLabel { get; set; }
    }
}
