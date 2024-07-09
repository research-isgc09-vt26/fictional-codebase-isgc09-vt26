using System;
using System.Collections.Generic;
using System.Text;

namespace SkillStarLearning.SubscriptionRules.Application.Contracts.Features.UpdateSubscriptionSettings
{
    public sealed class UpdateSubscriptionSettingsCommand
    {
        public required string UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string PreferredDisplayName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public required string BillingAddress { get; set; }
        public string LocalCommunityRegion { get; set; } = string.Empty;
        public bool AllowsEventCommunication { get; set; }
        public bool HasAcceptedMembershipTerms { get; set; }
        public string AccessibilityNotes { get; set; } = string.Empty;
        public string EmergencyContactPreference { get; set; } = string.Empty;
    }

}
