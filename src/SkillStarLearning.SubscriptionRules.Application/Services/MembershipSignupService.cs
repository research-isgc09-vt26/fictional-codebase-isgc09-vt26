using SkillStarLearning.SubscriptionRules.Application.Contracts.Features.CreateMembershipSignup;
using SkillStarLearning.SubscriptionRules.Application.Contracts.Infrastructure;
using SkillStarLearning.SubscriptionRules.Application.Contracts.Persistence;
using SkillStarLearning.SubscriptionRules.Application.Contracts.Services;
using SkillStarLearning.SubscriptionRules.Application.Exceptions;
using SkillStarLearning.SubscriptionRules.Application.Models;
using SkillStarLearning.SubscriptionRules.Core.Entities;
using SkillStarLearning.SubscriptionRules.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace SkillStarLearning.SubscriptionRules.Application.Services
{
    public sealed class MembershipSignupService : IMembershipSignupService
    {
        private readonly IMembershipSignupRepository _membershipSignupRepository;
        private readonly IAuditLogWriter _auditLogWriter;
        private readonly TimeProvider _timeProvider;
        private readonly IMarketSubscriptionPolicy _marketSubscriptionPolicy;


        public MembershipSignupService(
            IMembershipSignupRepository membershipSignupRepository,
            IAuditLogWriter auditLogWriter,
            IMarketSubscriptionPolicy marketSubscriptionPolicy,
            TimeProvider timeProvider)
        {
            _membershipSignupRepository = membershipSignupRepository;
            _auditLogWriter = auditLogWriter;
            _marketSubscriptionPolicy = marketSubscriptionPolicy;
            _timeProvider = timeProvider;
        }

        public async Task<MembershipSignupResultDto> StartOfflineEventSignupAsync(
            CreateMembershipSignupCommand command,
            CancellationToken cancellationToken = default)
        {
            if (!_marketSubscriptionPolicy.IsMembershipSignupAvailable(command.Segmentation))
            {
                throw new BusinessRuleException("MembershipSignup is not available in this market.");
            }

            var now = _timeProvider.GetUtcNow().UtcDateTime;
            var signup = new MembershipSignup
            {
                UserId = command.UserId,
                StaffMember = command.StaffMember,
                TrialStartsOn = now,
                TrialEndsOn = now.AddDays(30),
                CreatesPaidSubscription = command.CreatesPaidSubscription,
                CreatedBy = command.StaffMember,
                CreatedDate = now,
                Segmentation = command.Segmentation
            };

            await _membershipSignupRepository.AddAsync(signup, cancellationToken);
            await _auditLogWriter.WriteAsync($"MembershipSignup created by {command.StaffMember}.", cancellationToken);

            return new MembershipSignupResultDto
            {
                SignupId = signup.SignupId,
                UserId = signup.UserId,
                SignupType = signup.SignupType,
                TrialStatus = SubscriptionStatus.Trial,
                TrialStartsOn = signup.TrialStartsOn,
                TrialEndsOn = signup.TrialEndsOn,
                CreatesPaidSubscription = signup.CreatesPaidSubscription
            };
        }
    }
}
