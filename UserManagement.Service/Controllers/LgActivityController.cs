using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using Shared.Configurations.Attributes;
using Shared.Exceptions;
using Shared.Helpers;
using Shared.Helpers.HelperEnums;
using Shared.Helpers.HelperModels;
using Shared.Infrastructures.Repositories;
using Shared.Infrastructures.Services.User;
using Shared.Models.Dtos.Core;
using Shared.Models.Filters;
using System.Data;
using User_Management.Infrastructures.Services.AcitivityLogs;
using User_Management.Models;
using User_Management.Models.Requests;
using User_Management.Queries;

namespace User_Management.Controllers
{
    [Authorize]
    [Route("user/log-activity")]
    [ApiController]
    public class LgActivityController : ApiController
    {
        private readonly IConfiguration _configuration;
        private readonly IActivityService _activityService;
        private readonly IMapper _mapper;
        private readonly ICurrentUserService _currentUserService;
        private readonly string _conn;

        private readonly string _apiKey;

        public LgActivityController(IConfiguration configuration,
            IActivityService activityService,
            IMapper mapper,
            ICurrentUserService currentUserService, IHttpContextAccessor httpContextAccessor) : base(httpContextAccessor)
        {
            _configuration = configuration;
            _activityService = activityService;
            _mapper = mapper;
            _currentUserService = currentUserService;
            _conn = _configuration.GetConnectionString("DefaultConnection");
            _apiKey = _configuration.GetValue<string>("ApiKey");
        }
        [HttpGet("export")]
        public async Task<IActionResult> ExportAsync([FromQuery] LgActivityRequest request)
        {
            var fileName = $"ENS_Activity_Log_{DateTime.Now.ToString("ddMMyyyyhhmmss")}.xlsx";
            var bytes = await _activityService.ExportAsync(_mapper.Map<PagedDto>(request), _mapper.Map<ActivityFilter>(request));
            return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
        [HttpGet]
        public ListDataResponse Get([FromQuery] LgActivityRequest request)
        {
            string apiKey = string.Empty;


            ListDataResponse response = new();

            string listQuery = LgActivityQueries.getList;
            string countListQuery = LgActivityQueries.countList;

            Dictionary<string, string> data = new Dictionary<string, string>();

            if (request.page.Equals(0))
            {
                throw new InputRequiredException("The parameter page is required.");
            }
            else if (request.size.Equals(0))
            {
                throw new InputRequiredException("The parameter size is required.");
            }

            if (request.sort == null)
            {
                request.sort = "time desc";
            }

            List<FilterBuilderModel> param = new();
            if (request.username != null)
            {
                param.Add(new FilterBuilderModel("username", FilterBuilderEnum.LIKE, "'%" + request.username + "%'"));
            }

            if (request.startDate != null)
            {
                try
                {
                    _ = DateTime.Parse(request.startDate);
                }
                catch (Exception ex)
                {
                    throw new DateInvalidException("Format startDate invalid");
                }
                param.Add(new FilterBuilderModel("time", FilterBuilderEnum.GREATER_THAN_OR_EQUAL, "'" + request.startDate + "'"));
            }
            else
            {
                param.Add(new FilterBuilderModel("time", FilterBuilderEnum.GREATER_THAN_OR_EQUAL, "'1976-01-01'"));
            }

            if (request.endDate != null)
            {
                DateTime endDate;
                try
                {
                    endDate = DateTime.Parse(request.endDate);
                    endDate = endDate.Date.AddDays(1).AddMilliseconds(-1);
                }
                catch (Exception ex)
                {
                    throw new DateInvalidException("Format endDate invalid");
                }
                param.Add(new FilterBuilderModel("time", FilterBuilderEnum.LESS_THAN_OR_EQUAL, "'" + endDate.ToString("yyyy-MM-dd HH:mm:ss") + "'"));
            }
            else
            {
                DateTime endDate = DateTime.Now.Date.AddDays(1).AddMilliseconds(-1);

                param.Add(new FilterBuilderModel("time", FilterBuilderEnum.LESS_THAN_OR_EQUAL, "'" + endDate.ToString("yyyy-MM-dd HH:mm:ss") + "'"));
            }

            listQuery = DbHelper.filterBuilder(listQuery, param, FilterBuilderEnum.AND);
            listQuery = DbHelper.paginationBuilder(listQuery, request.page, request.size, request.sort);

            countListQuery = DbHelper.filterBuilder(countListQuery, param, FilterBuilderEnum.AND);

            try
            {
                DataTable table = DbHelper.executeQuery(_conn, listQuery);
                object total = DbHelper.executeQueryScalar(_conn, countListQuery);

                response.status = "Success";
                response.code = 200;
                response.message = "";
                response.data = new JsonResult(table).Value;
                response.total = (int)(long)total;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }


            return response;
        }

        [HttpPost]
        public DefaultResponse Post([FromBody] LgActivity activity)
        {
            string apiKey = string.Empty;
            if (Request.Headers.TryGetValue("ApiKey", out var traceValue))
            {
                apiKey = traceValue;
            }

            if (activity.username == null)
            {
                throw new InputRequiredException("The Username field is required.");
            }
            else if (activity.activity == null)
            {
                throw new InputRequiredException("The Activity field is required.");
            }

            DefaultResponse response = new();

            if (apiKey != string.Empty && apiKey.Equals(_apiKey))
            {
                if (_currentUserService.CurrentUser == null)
                    throw new ArgumentException("Login First");
                if (string.IsNullOrEmpty(activity.username))
                    activity.username = _currentUserService.CurrentUser.UserName;
                string insert = LgActivityQueries.insert;
                Dictionary<string, string> data = new Dictionary<string, string>();
                data.Add("@username", activity.username);
                data.Add("@activity", activity.activity);
                data.Add("@time", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));

                try
                {
                    DataTable table = DbHelper.executeQuery(_conn, insert, data);

                    response.status = "Success";
                    response.code = 200;
                    response.message = "Insert log activity successfull";
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
            else
            {
                throw new ApiKeyInvalidException("API Key Invalid");
            }

            return response;
        }
    }
}
