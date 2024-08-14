using Microsoft.AspNetCore.Mvc;
using Shared.Constants;
using Shared.Infrastructures.Repositories;
using Shared.Infrastructures.Repositories.Configuration;
using Shared.Models.Core;
using User_Management.Infrastructures.Services;
using User_Management.Infrastructures.Services.Role;
using User_Management.Models.Dtos.Role;
using User_Management.Models.Dtos.UserRole;

namespace User_Management.Controllers
{
    [Route("user/user-roles/import")]
    [ApiController]
    public class ImportRoleUserController : ApiController
    {
        private const char DELIMITER = ',';

        private const string FILE_EXTENSTIONS = ".csv";

        private const string MESSAGE_APPORVAL_NOTE = "Imported";

        private const string MESSAGE_COLUMN_NOT_MATCH = "Total Column not match";

        private const string MESSAGE_FILE_EXTENSIONS = "Please upload Csv Files";

        private const string MESSAGE_FILE_IS_EMTPY = "File is Empty";

        private const string MESSAGE_HEADER_IS_EMPTY = "Headers is Empty";

        private const string MESSAGE_STATUS_ROLE_NOT_FOUND = "Role '{0}' not found";

        private const string MESSAGE_STATUS_SUCCESS = "Success";

        private const string MESSAGE_STATUS_USER_NOT_FOUND = "User '{0}' not found";

        private const string MESSAGE_TOTAL_HEADERS_NOT_MATCH = "Please update csv file with headers (Role, Username & Is Approval)";

        private const string STATUS_HEADER = "Status";

        private const string MESSAGE_EXCEED_ROWS = "Exceed The Limit({0} User)";

        private const string CONTENT_TYPE = "text/csv";

        private readonly IConfigurationRepository _configurationRepository;

        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly ILogger<ImportRoleUserController> _logger;

        private readonly IRoleService _roleService;

        private readonly IRoleUserService _roleUserService;

        private readonly ISearchUserRepository _searchUserRepository;

        private List<string> listResult = new List<string>();
        private int totalLines;
        private int currentLine;

        public ImportRoleUserController(IHttpContextAccessor httpContextAccessor,
            IRoleService roleService,
            IRoleUserService roleUserService,
            ISearchUserRepository searchUserRepository,
            ILogger<ImportRoleUserController> logger,
            IConfigurationRepository configurationRepository)
            : base(httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _roleService = roleService;
            _roleUserService = roleUserService;
            _searchUserRepository = searchUserRepository;
            _logger = logger;
            _configurationRepository = configurationRepository;
        }

        [HttpPost()]
        public async Task<IActionResult> UploadDocumentGNSAsync()
        {
            var configuration = await _configurationRepository.GetActiveDirectory();
            if (configuration != null)
            {
                if (_httpContextAccessor.HttpContext != null && !_httpContextAccessor.HttpContext.Items.ContainsKey(ApiConstants.ActivityDirectoryConfiguration))
                    _httpContextAccessor.HttpContext.Items.Add(ApiConstants.ActivityDirectoryConfiguration, configuration);
            }
            var tempPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            try
            {
                var file = Request.Form.Files[0];
                using var memory = new MemoryStream();
                file.CopyTo(memory);
                CleanUpFile(tempPath);
                System.IO.File.WriteAllBytes(tempPath, memory.ToArray());
                var resultValidation = CheckFile(file, tempPath);
                if (resultValidation != null)
                    return resultValidation;

                await ImportAsync(tempPath);

                CreateResult(tempPath);

                return File(System.IO.File.ReadAllBytes(tempPath), CONTENT_TYPE, file.FileName);
            }
            finally
            {
                CleanUpFile(tempPath);
            }
        }

        private static void CleanUpFile(string tempPath)
        {
            if (System.IO.File.Exists(tempPath))
                System.IO.File.Delete(tempPath);
        }

        private async Task AddUseRole(string? createUniqueId, RoleDto role, User? user, bool isApproval, Paged<TmpRoleUserRefDto> userRoleRef)
        {
            if (userRoleRef.TotalItems == 0)
                await _roleUserService.AddRoleUserAsync(createUniqueId, new RoleUserDto()
                {
                    IsApprover = isApproval,
                    RoleId = role.Id,
                    Username = user.UserName
                });
        }

        private string BuildLine(List<string> lines)
        {
            return string.Join(DELIMITER, lines);
        }

        private IActionResult CheckFile(IFormFile file, string tempPath)
        {
            if (!file.FileName.EndsWith(FILE_EXTENSTIONS))
                return BadRequest(MESSAGE_FILE_EXTENSIONS);

            var lines = System.IO.File.ReadAllLines(tempPath);
            if (!lines.Any())
                return BadRequest(MESSAGE_FILE_IS_EMTPY);

            var headers = lines[0].Split(new[] { ',' });
            if (!headers.Any())
                return BadRequest(MESSAGE_HEADER_IS_EMPTY);

            if (headers.Count() != 3)
                return BadRequest(MESSAGE_TOTAL_HEADERS_NOT_MATCH);
            return null;
        }

        private void CreateResult(string tempPath)
        {
            CleanUpFile(tempPath);
            System.IO.File.WriteAllLines(tempPath, listResult.ToArray());
        }

        private async Task<RoleDto> GetRoleAsync(List<string> data)
        {
            var role = await _roleService.GetActiveByNameAsync(data[0]);
            if (role == null)
            {
                data.Add(String.Format(MESSAGE_STATUS_ROLE_NOT_FOUND, data[0]));
                listResult.Add(BuildLine(data));
            }
            return role;
        }

        private async Task<User?> GetUserAsync(List<string> data)
        {
            var user = await _searchUserRepository.GetById(data[1]);
            if (user == null)
            {
                data.Add(string.Format(MESSAGE_STATUS_USER_NOT_FOUND, data[1]));
                listResult.Add(BuildLine(data));
            }
            return user;
        }

        private async Task ImportAsync(string path)
        {
            var lines = System.IO.File.ReadAllLines(path);
            var headers = lines[0].Split(new[] { DELIMITER }).ToList();
            headers.Add(STATUS_HEADER);
            listResult.Add(BuildLine(headers));
            //var dictionary = new Dictionary<string, Role>();
            var groups = lines.Skip(1).GroupBy(n => n.Split(new[] { DELIMITER })[0]);
            totalLines = Convert.ToInt32(await _configurationRepository.GetValue(ApiConstants.IMPORT_LIMIT_ROW, ApiConstants.CONFIGURATION_GROUP) ?? "200");
            currentLine = 0;
            foreach (var group in groups)
            {
                _logger.LogInformation($"Prepare Import {group.Key} With {group.Count()} User");
                var roleUsers = await _roleUserService.GetUserRoleByName(group.Key);

                int? roleUserId = roleUsers != null ? roleUsers.Id : null;
                var createUniqueId = (await _roleUserService.CreateUniqueAsync(roleUserId, true))?.Data;
                var isValid = false;
                isValid = await IterateUser(group, createUniqueId, isValid);
                if (!isValid) continue;

                var result = await _roleUserService.CommitRoleUserAsync(createUniqueId, new Models.Dtos.UserRole.RoleUserDto()
                {
                    Id = roleUserId,
                    IsActive = true
                });
                await _roleUserService.ApprovedAsync(roleUserId.Value, Shared.Constants.ApiConstants.ActivityEnum.Approved, MESSAGE_APPORVAL_NOTE);
                if (totalLines == currentLine)
                {
                    listResult.Add(string.Format(MESSAGE_EXCEED_ROWS, totalLines));
                    break;
                }
            }
        }

        private async Task<bool> IterateUser(IGrouping<string, string> group, string? createUniqueId, bool isValid)
        {
            foreach (var item in group)
            {
                var data = item.Split(new[] { DELIMITER }).ToList();
                if (data.Count != 3)
                {
                    data.Add(MESSAGE_COLUMN_NOT_MATCH);
                    listResult.Add(BuildLine(data));
                    continue;
                }

                var role = await GetRoleAsync(data);
                if (role == null) continue;
                var user = await GetUserAsync(data);
                if (user == null) continue;

                var isApproval = data[2] == "Y" ? true : false;
                var userRoleRef = await _roleUserService.GetPagedRoleUserAsync(createUniqueId, new Shared.Models.Dtos.Core.PagedDto(), new Models.Filters.RoleUserFilter()
                {
                    Name = user.UserName,
                });
                await AddUseRole(createUniqueId, role, user, isApproval, userRoleRef);
                isValid = true;
                data.Add(MESSAGE_STATUS_SUCCESS);
                listResult.Add(BuildLine(data));
                currentLine++;
                if (totalLines == currentLine)
                    break;
            }

            return isValid;
        }
    }
}