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
    }
}
