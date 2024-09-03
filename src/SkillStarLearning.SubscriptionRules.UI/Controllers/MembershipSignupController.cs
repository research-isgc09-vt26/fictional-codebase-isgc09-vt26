using Microsoft.AspNetCore.Mvc;
using SkillStarLearning.SubscriptionRules.Application.Contracts.Features.CreateMembershipSignup;
using SkillStarLearning.SubscriptionRules.Application.Models;
using SkillStarLearning.SubscriptionRules.Application.Services;

namespace SkillStarLearning.SubscriptionRules.UI.Controllers
{
    [ApiController]
    [Route("api/membership-signups")]
    public sealed class MembershipSignupController : ControllerBase
    {
        private readonly MembershipSignupService _membershipSignupService;

        public MembershipSignupController(MembershipSignupService membershipSignupService)
        {
            _membershipSignupService = membershipSignupService;
        }

        [HttpPost(Name = "CreateMembershipSignup")]
        [ProducesResponseType(typeof(MembershipSignupResultDto), StatusCodes.Status200OK)]
        public async Task<ActionResult<MembershipSignupResultDto>> Create(
            [FromBody] CreateMembershipSignupCommand command,
            CancellationToken cancellationToken)
        {
            return Ok(await _membershipSignupService.StartOfflineEventSignupAsync(command, cancellationToken));
        }
    }
}
