using Dapper;
using Shared.Infrastructures.Repositories.EntityFramework;
using Shared.Models.Core;
using Shared.Models.Filters;
using User_Management.Models.Entities;
using User_Management.Queries;
using static Shared.Constants.ApiConstants;

namespace User_Management.Infrastructures.Repositories.Role
{
    public class EFRoleHistoryRepostitory : EFBaseRepository<LgRole>, IRoleHisotryRepository
    {
        public EFRoleHistoryRepostitory(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            IsAuditEntity = false;
        }

        public Task<LgRole> AddAsync(MdRole role, ActivityEnum activity, string note = null)
        {
            var history = new LgRole()
            {
                Activity = activity.ToString(),
                Note = note,
                RoleId = role.Id,
                User = _currentUserService.CurrentUser?.UserName,
                Name = role.Name,
                IsAdmin = role.IsAdmin,
                IsActive = role.IsActive,
                Description =role.Description
            };
            return base.AddAsync(history);
        }

        public async Task<Tuple<bool, ActivityEnum>> CheckHasApproveOrReject(int id)
        {
            using (var connection = OpenConnection())
            {
                var role = await GetLatestByParentId(id);
                if (role.Activity != ActivityEnum.Submitted.ToString())
                {
                    return new(true, (ActivityEnum)Enum.Parse(typeof(ActivityEnum), role.Activity));
                }
                else
                {
                    return new(false, ActivityEnum.Submitted);
                }
            }
        }

        public Task<LgRole> GetLatestByParentId(int id)
        {
            NoTraciking = true;
            return Task.FromResult(GetAll().Where(n => n.RoleId == id).OrderBy(n => n.Id).LastOrDefault());
        }

        public Task<LgRole> GetLatestLogByParent(int id)
        {
            return Task.FromResult(GetAll().OrderByDescending(n => n.Id).FirstOrDefault(n => n.RoleId == id));
        }

        public Task<Paged<LgRole>> GetPaged(int roleId, PageFilter page)
        {
            return base.GetPaged(page, n => n.RoleId == roleId);
        }
    }
}