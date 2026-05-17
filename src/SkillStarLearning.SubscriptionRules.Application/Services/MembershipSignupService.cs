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
        private readonly ISubscriptionMessageService _subscriptionMessageService;

        public MembershipSignupService(
            IMembershipSignupRepository membershipSignupRepository,
            IAuditLogWriter auditLogWriter,
            IMarketSubscriptionPolicy marketSubscriptionPolicy,
            ISubscriptionMessageService subscriptionMessageService,
            TimeProvider timeProvider)
        {
            _membershipSignupRepository = membershipSignupRepository;
            _auditLogWriter = auditLogWriter;
            _marketSubscriptionPolicy = marketSubscriptionPolicy;
            _subscriptionMessageService = subscriptionMessageService;
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
            var trialOffered = _marketSubscriptionPolicy.IsMembershipSignupTrialOffered(command.Segmentation);
            var trialEndsOn = trialOffered ? now.AddDays(30) : now;

            var signup = new MembershipSignup
            {
                UserId = command.UserId,
                StaffMember = command.StaffMember,
                TrialStartsOn = now,
                TrialEndsOn = trialEndsOn,
                CreatesPaidSubscription = command.CreatesPaidSubscription,
                CreatedBy = command.StaffMember,
                CreatedDate = now,
                Segmentation = command.Segmentation
            };

            await _membershipSignupRepository.AddAsync(signup, cancellationToken);
            await _auditLogWriter.WriteAsync($"MembershipSignup created by {command.StaffMember}.", cancellationToken);

            var message = _subscriptionMessageService.GetMessage(SubscriptionMessageFlowType.MembershipSignup, command.Segmentation);

            return new MembershipSignupResultDto
            {
                SignupId = signup.SignupId,
                UserId = signup.UserId,
                SignupType = signup.SignupType,
                TrialStatus = trialOffered ? SubscriptionStatus.Trial : SubscriptionStatus.None,
                TrialStartsOn = signup.TrialStartsOn,
                TrialEndsOn = signup.TrialEndsOn,
                CreatesPaidSubscription = signup.CreatesPaidSubscription,
                CustomerMessage = message.CustomerText
            };
        }
    }
}
