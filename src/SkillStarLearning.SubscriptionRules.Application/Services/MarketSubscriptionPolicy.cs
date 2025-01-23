using SkillStarLearning.SubscriptionRules.Application.Contracts.Services;
using SkillStarLearning.SubscriptionRules.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SkillStarLearning.SubscriptionRules.Application.Services
{
    public sealed class MarketSubscriptionPolicy : IMarketSubscriptionPolicy
    {
        public MarketSubscriptionModel GetSubscriptionModel(Segmentation segmentation)
        {
            return segmentation switch
            {
                Segmentation.SegmentationA => MarketSubscriptionModel.StandardMixedSubscriptionModel,
                Segmentation.SegmentationB => MarketSubscriptionModel.CommunityOnlySubscriptionModel,
                _ => throw new ArgumentOutOfRangeException(nameof(segmentation), segmentation, "Unsupported market.")
            };
        }

        public bool IsSubscriptionAvailable(Segmentation segmentation, SubscriptionType subscriptionType)
        {
            var model = GetSubscriptionModel(segmentation);

            var isCommunityOnlySubscriptionModel = subscriptionType == SubscriptionType.CommunityMembershipSubscription;

            return model switch
            {
                MarketSubscriptionModel.StandardMixedSubscriptionModel => true,
                MarketSubscriptionModel.CommunityOnlySubscriptionModel => isCommunityOnlySubscriptionModel,
                _ => false
            };
        }

        public bool IsMembershipSignupAvailable(Segmentation segmentation)
        {
            return GetSubscriptionModel(segmentation) == MarketSubscriptionModel.StandardMixedSubscriptionModel;
        }
    }
}
