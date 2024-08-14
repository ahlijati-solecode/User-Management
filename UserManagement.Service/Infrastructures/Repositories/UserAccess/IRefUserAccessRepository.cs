using Shared.Infrastructures.Repositories.EntityFramework;
using Shared.Models.Core;
using Shared.Models.Filters;
using User_Management.Models.Entities;

namespace User_Management.Infrastructures.Repositories.UserAccess
{
    public class EFUserAccessLogRepository : EFBaseRepository<LgRoleAccess>, IRoleAccessLogRepository
    {
        public EFUserAccessLogRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public Task<LgRoleAccess> GetLatestByUserAccessId(int id)
        {
            NoTraciking = true;
            return Task.FromResult(GetAll().Where(n => n.ParentId == id).OrderBy(n => n.Id).LastOrDefault());
        }

        public async Task<IEnumerable<LgRoleAccess>> GetLogs(int id)
        {
            NoTraciking = true;
            return GetAll().Where(n => n.ParentId == id).OrderBy(n => n.Id).ToList();
        }

        public Task<Paged<LgRoleAccess>> GetPaged(int id, PageFilter page)
        {
            return base.GetPaged(page, n => n.ParentId == id);
        }

    }
}
