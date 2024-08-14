using Microsoft.AspNetCore.Mvc;
using Shared.Configurations.Attributes;
using Shared.Constants;
using User_Management.Constants;
using static Shared.Constants.ApiConstants;

namespace User_Management.Controllers
{
    [Authorize]
    [Route("user/status")]
    [ApiController]
    public class StatusController : ApiController
    {
        public StatusController(IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
        }

        [HttpGet("role")]
        public IActionResult GetStatusRole()
        {
            var type = typeof(RoleConstants.StatusEnum);
            return Ok(Enum.GetValues<RoleConstants.StatusEnum>().Select(n => new { key = n, value = Enum.GetName(type, n) }));
        }

        [HttpGet("approval-role")]
        public IActionResult GetApprovalStatusRole()
        {
            var type = typeof(ActivityEnum);
            return Ok(Enum.GetValues<ActivityEnum>()
                .Select(n => new { key = n, value = Enum.GetName(type, n) }));
        }
    }
}