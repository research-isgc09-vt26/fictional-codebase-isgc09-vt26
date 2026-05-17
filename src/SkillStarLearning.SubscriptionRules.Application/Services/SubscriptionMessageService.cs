using SkillStarLearning.SubscriptionRules.Application.Contracts.Services;
using SkillStarLearning.SubscriptionRules.Application.Models;
using SkillStarLearning.SubscriptionRules.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SkillStarLearning.SubscriptionRules.Application.Services
{
    public sealed class SubscriptionMessageService : ISubscriptionMessageService
    {
        public SubscriptionMessageDto GetMessage(SubscriptionMessageFlowType flowType, Segmentation segmentation)
        {
            if(segmentation == Segmentation.SegmentationB)
            {
                return flowType switch
                {
                    SubscriptionMessageFlowType.MembershipSignup => new SubscriptionMessageDto
                    {
                        FlowType = flowType,
                        CustomerText = "Your community subscription is ready.",
                        RefersToMembershipSignup = true
                    },
                    _ => new SubscriptionMessageDto
                    {
                        FlowType = flowType,
                        CustomerText = "Your community subscription is activated.",
                        RefersToMembershipSignup = false
                    }
                };
            }

            var defaultMessage = new SubscriptionMessageDto
            {
                FlowType = flowType,
                RefersToMembershipSignup = false
            };

            return flowType switch
            {
                SubscriptionMessageFlowType.OnlineSubscription => defaultMessage,
                SubscriptionMessageFlowType.CommunityMembershipSubscription => defaultMessage,
                SubscriptionMessageFlowType.MembershipSignup => new SubscriptionMessageDto
                {
                    FlowType = flowType,
                    CustomerText = "Your community signup is complete.",
                    RefersToMembershipSignup = true
                },
                _ => throw new ArgumentOutOfRangeException(nameof(flowType), flowType, "Unsupported subscription flow.")
            };
        }
    }
}
