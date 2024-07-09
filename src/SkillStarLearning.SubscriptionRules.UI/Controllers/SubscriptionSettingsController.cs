using Microsoft.AspNetCore.Mvc;
using SkillStarLearning.SubscriptionRules.Application.Contracts.Features.UpdateSubscriptionSettings;
using SkillStarLearning.SubscriptionRules.Application.Models;
using SkillStarLearning.SubscriptionRules.Application.Services;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace SkillStarLearning.SubscriptionRules.UI.Controllers
{
    [ApiController]
    [Route("api/subscription-settings")]
    public sealed class SubscriptionSettingsController : ControllerBase
    {
        private readonly OldSubscriptionService _subscriptionService;
        private readonly IUpdateSubscriptionSettingsHandler _updateSubscriptionSettingsHandler;

        public SubscriptionSettingsController(
            OldSubscriptionService subscriptionService,
            IUpdateSubscriptionSettingsHandler updateSubscriptionSettingsHandler)
        {
            _subscriptionService = subscriptionService;
            _updateSubscriptionSettingsHandler = updateSubscriptionSettingsHandler;
        }

        [HttpGet("{userId}", Name = "GetSubscriptionSettings")]
        [ProducesResponseType(typeof(SubscriptionOverviewDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<SubscriptionOverviewDto>> Get(string userId, CancellationToken cancellationToken)
        {
            return Ok(await _subscriptionService.GetSubscriptionSettingsAsync(userId, cancellationToken));
        }

        [HttpPut("{userId}", Name = "UpdateSubscriptionSettings")]
        [ProducesResponseType(typeof(SubscriptionOverviewDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<SubscriptionOverviewDto>> Update(
        string userId,
        [FromBody] UpdateSubscriptionSettingsCommand command,
        CancellationToken cancellationToken)
        {
            command.UserId = userId;
            return Ok(await _updateSubscriptionSettingsHandler.Handle(command, cancellationToken));
        }
    }
}
