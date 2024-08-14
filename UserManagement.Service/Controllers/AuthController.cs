using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Shared.Configurations;
using Shared.Configurations.Attributes;
using Shared.Infrastructures.Repositories;
using Shared.Models.Core;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using User_Management.Constants;
using User_Management.Infrastructures.Services;
using User_Management.Models.Requests.Auth;
using User_Management.Models.Responses.RoleAccess;

namespace User_Management.Controllers
{
    [Route("user/auth")]
    [ApiController]
    public class AuthController : ApiController
    {
        private readonly IUserService _userService;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUnitOfWorks _unitOfWorks;
        private readonly IMapper _mapper;
        private readonly AppSettings _appSettings;

        public AuthController(IUserService userService,
            IHttpContextAccessor httpContextAccessor,
            IOptions<AppSettings> appSettings,
            IUnitOfWorks unitOfWorks,
            IMapper mapper) : base(httpContextAccessor)
        {
            _userService = userService;
            _httpContextAccessor = httpContextAccessor;
            _unitOfWorks = unitOfWorks;
            _mapper = mapper;
            _appSettings = appSettings.Value;
        }

        [Authorize]
        [HttpGet("current")]
        public async Task<IActionResult> GetAsync()
        {
            var result = await _userService.GetAccessControl();
            switch (result.State)
            {
                case UserConstants.AccessCotntrolListState.UserNotFound:
                    Message = UserConstants.InvalidUserNameMessage;
                    return NotFound(UserConstants.InvalidUserNameMessage);
            }
            return Ok(_mapper.Map<List<UserAccessControlResponse>>(result.Data));
        }

        [HttpPost("login")]
        public async Task<IActionResult> PostAsync([FromBody] LoginRequest request)
        {
            var result = await _userService.LoginAsync(request.UserName, request.Password);
            switch (result.State)
            {
                case UserConstants.LoginStateEnum.InvalidUsername:
                    Message = UserConstants.InvalidUserNameMessage;
                    return NotFound(UserConstants.InvalidUserNameMessage);

                case UserConstants.LoginStateEnum.ActiveDirectoryNotConfigure:
                    return Problem(UserConstants.ActivityDirectoryNotConfigureMessage);
            }
            Message = UserConstants.LoginSuccessMessage;
            return Ok(new { UserToken = GenerateJwtToken(result.Data, request.IsKeepLogged) });
        }

        private string GenerateJwtToken(User user, bool isKeepLogged)
        {
            if (user == null)
                throw new ArgumentNullException($"{nameof(user)} is Null");
            // generate token that is valid for 7 days
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] { new Claim("id", user.UserName.ToString()) }),
                Expires = isKeepLogged ? DateTime.UtcNow.AddYears(7) : DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            tokenDescriptor.Claims = new Dictionary<string, object>();
            tokenDescriptor.Claims.Add("roles", user.Roles);
            tokenDescriptor.Claims.Add("email", user.Email);
            tokenDescriptor.Claims.Add("fullname", user.FullName);
            tokenDescriptor.Claims.Add("departmentName", user.Departement);
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
