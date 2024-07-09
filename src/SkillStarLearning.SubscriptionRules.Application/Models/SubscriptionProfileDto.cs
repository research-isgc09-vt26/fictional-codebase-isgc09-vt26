namespace SkillStarLearning.SubscriptionRules.Application.Models
{
    public sealed class SubscriptionProfileDto
    {
        public required string UserId { get; set; }
        public string PreferredDisplayName { get; set; } = string.Empty;
        public required string Email { get; set; }
        public required string BillingAddress { get; set; }
        public bool HasAcceptedMembershipTerms { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string LocalCommunityRegion { get; set; } = string.Empty;
        public bool AllowsEventCommunication { get; set; }
        public string AccessibilityNotes { get; set; } = string.Empty;
        public string EmergencyContactPreference { get; set; } = string.Empty;
    }
}
