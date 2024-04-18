using Microsoft.AspNetCore.Mvc;
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

        public SubscriptionSettingsController(
            OldSubscriptionService subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }

        [HttpGet("{userId}", Name = "GetSubscriptionSettings")]
        [ProducesResponseType(typeof(SubscriptionOverviewDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<SubscriptionOverviewDto>> Get(string userId, CancellationToken cancellationToken)
        {
            return Ok(await _subscriptionService.GetSubscriptionSettingsAsync(userId, cancellationToken));
        }
    }
}
