using Microsoft.AspNetCore.Mvc;
using Shared.Service.Controllers;

namespace User_Management.Controllers
{
    [Route("user/logs")]
    [ApiController]
    public class LogsController : BaseLogController
    {
        public LogsController(IWebHostEnvironment webHostEnvironment, IHttpContextAccessor httpContextAccessor) : base(webHostEnvironment, httpContextAccessor)
        {
        }
    }
}