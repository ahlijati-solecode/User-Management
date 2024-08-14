using Shared.Infrastructures.Repositories.EntityFramework;
using Shared.Models.Core;
using Shared.Models.Filters;
using User_Management.Models.Entities;

namespace User_Management.Infrastructures.Repositories.RoleUser
{
    public class EFLogRoleUserRepository : EFBaseRepository<LgRoleUser>, ILogRoleUserRepository
    {
        public EFLogRoleUserRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            IsAuditEntity = false;
        }
        public Task<LgRoleUser> GetLatestLogByParentId(int id)
        {
            Includes(nameof(LgRoleUser.LgRoleUserRefs));
            NoTraciking = true;
            var items = GetAll().Where(n => n.ParentId == id).OrderBy(n => n.Id);
            return Task.FromResult(items.LastOrDefault());
        }

        public Task<Paged<LgRoleUser>> GetPaged(int id, PageFilter page)
        {
            return base.GetPaged(page, n => n.ParentId == id);
        }
    }

}
