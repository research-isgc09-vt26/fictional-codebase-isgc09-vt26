using SkillStarLearning.SubscriptionRules.Core.Enums;

namespace SkillStarLearning.SubscriptionRules.Application.Models
{
    public sealed class SubscriptionOverviewDto
    {
        public required string UserId { get; set; }
        public SubscriptionType SubscriptionType { get; set; }
        public SubscriptionStatus Status { get; set; }
        public DateTime? RenewalDate { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public required SubscriptionProfileDto Profile { get; set; }
        public MembershipSignupResultDto? SignupInfo { get; set; }
        public bool CanManageSubscription { get; set; }
        public bool RequiresMembershipProfileReview { get; set; }
    }
}
