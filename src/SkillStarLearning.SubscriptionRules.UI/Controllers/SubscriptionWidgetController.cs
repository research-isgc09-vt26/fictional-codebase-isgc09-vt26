using Microsoft.AspNetCore.Mvc;
using SkillStarLearning.SubscriptionRules.Application.Models;
using SkillStarLearning.SubscriptionRules.Application.Services;

namespace SkillStarLearning.SubscriptionRules.UI.Controllers
{
    [ApiController]
    [Route("api/subscription-widget")]
    public sealed class SubscriptionWidgetController : ControllerBase
    {
        private readonly NewSubscriptionService _newSubscriptionService;

        public SubscriptionWidgetController(NewSubscriptionService newSubscriptionService)
        {
            _newSubscriptionService = newSubscriptionService;
        }

        [HttpGet("{userId}", Name = "GetSubscriptionWidgetSummary")]
        [ProducesResponseType(typeof(SimpleSubscriptionSummaryDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<SimpleSubscriptionSummaryDto>> Get(string userId, CancellationToken cancellationToken)
        {
            return Ok(await _newSubscriptionService.GetSubscriptionWidgetSummaryAsync(userId, cancellationToken));
        }

        [HttpGet("{userId}/extended", Name = "GetExtendedSubscriptionWidget")]
        [ProducesResponseType(typeof(SubscriptionOverviewDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<SubscriptionOverviewDto>> GetExtended(string userId, CancellationToken cancellationToken)
        {
            return Ok(await _newSubscriptionService.GetExtendedSubscriptionWidgetAsync(userId, cancellationToken));
        }
    }
}
