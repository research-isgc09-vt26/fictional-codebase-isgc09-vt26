using SkillStarLearning.SubscriptionRules.Application.Contracts.Services;
using SkillStarLearning.SubscriptionRules.Application.Models;
using SkillStarLearning.SubscriptionRules.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SkillStarLearning.SubscriptionRules.Application.Services
{
    public sealed class SubscriptionAvailabilityService : ISubscriptionAvailabilityService
    {
        private readonly MarketSubscriptionPolicy _marketSubscriptionPolicy;

        public SubscriptionAvailabilityService(MarketSubscriptionPolicy marketSubscriptionPolicy)
        {
            _marketSubscriptionPolicy = marketSubscriptionPolicy;
        }

        public SubscriptionAvailabilityDto GetAvailability(Segmentation segmentation)
        {
            return new SubscriptionAvailabilityDto
            {
                Segmentation = segmentation,
                SubscriptionModel = _marketSubscriptionPolicy.GetSubscriptionModel(segmentation),
                OnlineSubscriptionAvailable = _marketSubscriptionPolicy.IsSubscriptionAvailable(segmentation, SubscriptionType.OnlineSubscription),
                CommunityMembershipSubscriptionAvailable = _marketSubscriptionPolicy.IsSubscriptionAvailable(segmentation, SubscriptionType.CommunityMembershipSubscription),
                MembershipSignupAvailable = _marketSubscriptionPolicy.IsMembershipSignupAvailable(segmentation)
            };
        }
    }
}
