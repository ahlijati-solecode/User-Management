using Shared.Models.Core;
using Shared.Models.Filters;
using User_Management.Constants;
using User_Management.Models.Entities;
using static Shared.Constants.ApiConstants;

namespace User_Management.Infrastructures.Repositories
{
    public interface IRoleHisotryRepository
    {
        Task<LgRole> AddAsync(MdRole role, ActivityEnum activity, string note = null);
        Task<LgRole> AddAsync(LgRole role);

        Task<Tuple<bool, ActivityEnum>> CheckHasApproveOrReject(int id);

        Task<Paged<LgRole>> GetPaged(int roleId, PageFilter page);
        Task<LgRole> GetLatestByParentId(int id);
        Task<LgRole> GetLatestLogByParent(int id);
    }
}