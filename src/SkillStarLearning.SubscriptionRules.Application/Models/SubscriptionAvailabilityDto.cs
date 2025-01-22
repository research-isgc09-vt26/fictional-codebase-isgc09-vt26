using SkillStarLearning.SubscriptionRules.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SkillStarLearning.SubscriptionRules.Application.Models
{
    public sealed class SubscriptionAvailabilityDto
    {
        public Segmentation Segmentation { get; set; }
        public MarketSubscriptionModel SubscriptionModel { get; set; }
        public bool OnlineSubscriptionAvailable { get; set; }
        public bool CommunityMembershipSubscriptionAvailable { get; set; }
        public bool MembershipSignupAvailable { get; set; }
    }
}
