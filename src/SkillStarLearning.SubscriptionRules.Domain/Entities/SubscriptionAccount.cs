using SkillStarLearning.SubscriptionRules.Domain.Common;
using SkillStarLearning.SubscriptionRules.Domain.Enums;

namespace SkillStarLearning.SubscriptionRules.Domain.Entities
{
    public sealed class SubscriptionAccount : AuditableEntity
    {
        public required string UserId { get; set; }
        public SubscriptionType SubscriptionType { get; set; }
        public SubscriptionStatus Status { get; set; }
        public DateTime? RenewalDate { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
    }
}
