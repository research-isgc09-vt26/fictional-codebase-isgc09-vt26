using SkillStarLearning.SubscriptionRules.Domain.Common;

namespace SkillStarLearning.SubscriptionRules.Domain.Entities
{
    public sealed class UserProfile : AuditableEntity
    {
        public required string UserId { get; set; }
        public string PreferredDisplayName { get; set; } = string.Empty;
        public required string Email { get; set; }
        public required string BillingAddress { get; set; }
        public bool HasAcceptedMembershipTerms { get; set; }
    }
}
