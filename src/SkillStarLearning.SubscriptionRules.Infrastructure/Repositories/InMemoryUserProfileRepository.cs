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
                PhoneNumber = "+45 11 22 33 44",
                BillingAddress = "Online Street 10, 1000 Copenhagen",
                HasAcceptedMembershipTerms = true
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
