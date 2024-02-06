namespace SkillStarLearning.SubscriptionRules.Application.Models
{
    public sealed class SubscriptionProfileDto
    {
        public required string UserId { get; set; }
        public string PreferredDisplayName { get; set; } = string.Empty;
        public required string Email { get; set; }
        public required string BillingAddress { get; set; }
        public bool HasAcceptedMembershipTerms { get; set; }
    }
}
