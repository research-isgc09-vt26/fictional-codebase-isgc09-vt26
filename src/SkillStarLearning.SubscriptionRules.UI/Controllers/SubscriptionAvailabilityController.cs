using Microsoft.AspNetCore.Mvc;
using SkillStarLearning.SubscriptionRules.Application.Models;
using SkillStarLearning.SubscriptionRules.Application.Services;
using SkillStarLearning.SubscriptionRules.Core.Enums;

namespace SkillStarLearning.SubscriptionRules.UI.Controllers
{
    [ApiController]
    [Route("api/markets/{segmentation}/subscription-availability")]
    public sealed class SubscriptionAvailabilityController : ControllerBase
    {
        private readonly SubscriptionAvailabilityService _subscriptionAvailabilityService;

        public SubscriptionAvailabilityController(SubscriptionAvailabilityService subscriptionAvailabilityService)
        {
            _subscriptionAvailabilityService = subscriptionAvailabilityService;
        }

        [HttpGet(Name = "GetSubscriptionAvailability")]
        [ProducesResponseType(typeof(SubscriptionAvailabilityDto), StatusCodes.Status200OK)]
        public ActionResult<SubscriptionAvailabilityDto> Get(Segmentation segmentation)
        {
            return Ok(_subscriptionAvailabilityService.GetAvailability(segmentation));
        }
    }
}
