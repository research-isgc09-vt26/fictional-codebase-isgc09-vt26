using Microsoft.AspNetCore.Mvc;
using SkillStarLearning.SubscriptionRules.Application.Models;
using SkillStarLearning.SubscriptionRules.Application.Services;
using SkillStarLearning.SubscriptionRules.Core.Enums;

namespace SkillStarLearning.SubscriptionRules.UI.Controllers
{
    [ApiController]
    [Route("api/subscription-message")]
    public sealed class SubscriptionMessageController : ControllerBase
    {
        private readonly SubscriptionMessageService _subscriptionMessageService;

        public SubscriptionMessageController(SubscriptionMessageService subscriptionMessageService)
        {
            _subscriptionMessageService = subscriptionMessageService;
        }

        [HttpGet("{flowType}", Name = "GetSubscriptionMessage")]
        [ProducesResponseType(typeof(SubscriptionMessageDto), StatusCodes.Status200OK)]
        public ActionResult<SubscriptionMessageDto> Get(
            SubscriptionMessageFlowType flowType,
            [FromQuery] Segmentation segmentation = Segmentation.SegmentationA)
        {
            return Ok(_subscriptionMessageService.GetMessage(flowType, segmentation));
        }
    }
}
