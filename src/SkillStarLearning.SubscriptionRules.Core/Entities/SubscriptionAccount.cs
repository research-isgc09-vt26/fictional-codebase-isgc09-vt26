using SkillStarLearning.SubscriptionRules.Core.Common;
using SkillStarLearning.SubscriptionRules.Core.Enums;

namespace SkillStarLearning.SubscriptionRules.Core.Entities
{
    public sealed class SubscriptionAccount : AuditableEntity
    {
        public required string UserId { get; set; }
        public SubscriptionType SubscriptionType { get; set; }
        public SubscriptionStatus Status { get; set; }
        public DateTime? RenewalDate { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public bool CanManageSubscription { get; set; }
    }
}
