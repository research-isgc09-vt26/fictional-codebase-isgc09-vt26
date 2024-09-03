using System;
using System.Collections.Generic;
using System.Text;

namespace SkillStarLearning.SubscriptionRules.Application.Contracts.Features.CreateMembershipSignup
{
    public sealed class CreateMembershipSignupCommand
    {
        public required string UserId { get; set; }
        public required string StaffMember { get; set; }
        public bool CreatesPaidSubscription { get; set; } = false;
    }

}
