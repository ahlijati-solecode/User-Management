using Shared.Infrastructures.Repositories;
using Shared.Models.Core;
using Shared.Models.Filters;
using User_Management.Models.Entities;
using User_Management.Models.Filters;

namespace User_Management.Infrastructures.Repositories.RoleUser
{
    public interface ITemporaryRoleUserRefRepository : IBaseRepository<TmpRoleUserRef, string>
    {
        Task<Paged<TmpRoleUserRef>> GetPaged(string uniqueId, PageFilter page, RoleUserFilter filter);
        Task<bool> IsDuplicate(string uniqueId, string userName);
    }

}
