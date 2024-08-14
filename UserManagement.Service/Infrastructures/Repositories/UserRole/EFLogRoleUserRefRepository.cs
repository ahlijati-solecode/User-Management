using Shared.Infrastructures.Repositories.EntityFramework;
using User_Management.Models.Entities;

namespace User_Management.Infrastructures.Repositories.RoleUser
{
    public class EFLogRoleUserRefRepository : EFBaseRepository<LgRoleUserRef>, ILogRoleUserRefRepository
    {
        public EFLogRoleUserRefRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            IsAuditEntity = false;
        }

        public Task<List<LgRoleUserRef>> GetUsers(int id, int count)
        {
            NoTraciking = true;
            return Task.FromResult(GetAll().Where(n => n.ParentId == id).OrderBy(n => n.Id).Take(count).ToList());
        }
    }

}
