using SkillStarLearning.SubscriptionRules.Application.Contracts.Persistence;
using SkillStarLearning.SubscriptionRules.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace SkillStarLearning.SubscriptionRules.Infrastructure.Repositories
{
    public sealed class InMemoryUserProfileRepository : IUserProfileRepository
    {
        private readonly Dictionary<string, UserProfile> _fakeProfiles = new(StringComparer.OrdinalIgnoreCase)
        {
            ["online-user-01"] = new UserProfile
            {
                UserId = "online-user-01",
                PreferredDisplayName = "Jane",
                Email = "jane.doe@example.test",
                BillingAddress = "Online Street 10, 1000 Copenhagen",
                HasAcceptedMembershipTerms = true,
                CreatedDate = DateTime.UtcNow.Date.AddDays(-30)
            },
            ["community-user-01"] = new UserProfile
            {
                UserId = "community-user-01",
                FullName = "John Doe",
                PreferredDisplayName = "Jonny",
                Email = "john.doe@example.test",
                PhoneNumber = "+45 55 66 77 88",
                BillingAddress = "Member Road 42, 2200 Copenhagen",
                LocalCommunityRegion = "Copenhagen North",
                AllowsEventCommunication = true,
                HasAcceptedMembershipTerms = true,
                CreatedDate = DateTime.UtcNow.Date.AddDays(-3),
                AccessibilityNotes = "Step-free access requested",
                EmergencyContactPreference = "Call listed phone number: +45 11 22 33 44 (Jane)"
            },
            ["community-user-02"] = new UserProfile
            {
                UserId = "community-user-01",
                FullName = "Jon Doe",
                PreferredDisplayName = "Jonny",
                Email = "jon.doe@example.test",
                PhoneNumber = "+45 55 66 77 89",
                BillingAddress = "Member Road 41, 2200 Copenhagen",
                LocalCommunityRegion = "Copenhagen North",
                AllowsEventCommunication = false,
                HasAcceptedMembershipTerms = true,
                CreatedDate = DateTime.UtcNow.Date.AddDays(-3),
                AccessibilityNotes = "",
                EmergencyContactPreference = ""
            }
        };

        public Task<UserProfile?> GetByUserIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            _fakeProfiles.TryGetValue(userId, out var profile);
            return Task.FromResult(profile);
        }

        public Task SaveAsync(UserProfile profile, CancellationToken cancellationToken = default)
        {
            _fakeProfiles[profile.UserId] = profile;
            return Task.CompletedTask;
        }
    }
}
