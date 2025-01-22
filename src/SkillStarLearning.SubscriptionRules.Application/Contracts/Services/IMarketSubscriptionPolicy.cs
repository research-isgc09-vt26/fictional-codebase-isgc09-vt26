using SkillStarLearning.SubscriptionRules.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SkillStarLearning.SubscriptionRules.Application.Contracts.Services
{
    public interface IMarketSubscriptionPolicy
    {
        MarketSubscriptionModel GetSubscriptionModel(Segmentation segmentation);
        public bool IsSubscriptionAvailable(Segmentation segmentation, SubscriptionType subscriptionType);
        public bool IsMembershipSignupAvailable(Segmentation segmentation);
    }
}
