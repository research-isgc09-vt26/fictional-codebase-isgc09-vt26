using SkillStarLearning.SubscriptionRules.Application.Models;
using SkillStarLearning.SubscriptionRules.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SkillStarLearning.SubscriptionRules.Application.Contracts.Services
{
    internal interface ISubscriptionMessageService
    {
        SubscriptionMessageDto GetMessage(SubscriptionMessageFlowType flowType);
    }
}
