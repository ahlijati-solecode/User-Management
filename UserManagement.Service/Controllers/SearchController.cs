using Microsoft.AspNetCore.Mvc;
using Shared.Configurations.Attributes;
using Shared.Constants;
using Shared.Infrastructures.Services;
using Shared.Models.Core;
using Shared.Models.Requests;
using User_Management.Constants;
using User_Management.Infrastructures.Repositories;
using User_Management.Infrastructures.Services;

namespace User_Management.Controllers
{
    [Authorize]
    [Route("user/search")]
    [ApiController]
    public class SearchController : ApiController
    {
        private readonly IUserService _userService;

        public SearchController(IUserService userService, IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
            _userService = userService;
        }
        [HttpGet()]
        public async Task<IActionResult> SearchUser([FromQuery] string? key, [FromQuery] int? page = 1, [FromQuery] int? size = 10, [FromQuery] string? sort = "userName asc",
            [FromQuery] int captureLog = 0)
        {
            if (captureLog == 1)
                LdapConsoleMufgUserRepository.IsCaptureResponse = captureLog == 1;
            var result = await _userService.Search(key);
            switch (result.State)
            {
                case UserConstants.SearchStateEnum.ActiveDirectoryNotConfigure:
                    return NotFound(UserConstants.ActivityDirectoryNotConfigureMessage);
                default:
                    break;
            }
            var query = result.Data.Where(n => !string.IsNullOrEmpty(n.UserName)).AsQueryable();
            query = query.OrderBy<User>(sort);
            //query = query.Skip((page.Value - 1) * size.Value).Take(size.Value);
            return Ok(query.ToList());
        }
    }
}