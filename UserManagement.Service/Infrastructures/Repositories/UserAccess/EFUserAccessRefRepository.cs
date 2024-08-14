using Microsoft.EntityFrameworkCore;
using Shared.Infrastructures.Repositories.EntityFramework;
using User_Management.Infrastructures.Repositories.UserAccess;
using User_Management.Models.Entities;

namespace User_Management.Infrastructures.Repositories.UserAccess
{
    public class EFUserAccessRefRepository : EFBaseRepository<MdRoleAccessRef>, IRoleAccessRefRepository
    {
        public EFUserAccessRefRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public Task<List<MdRoleAccessRef>> GetByRoles(int[] rolesId)
        {
            NoTraciking = true;
            var userRoles = GetAll()
                .Include(n => n.RefMenu)
                .Include(n => n.RefUserAccessNavigation)
                .Where(n =>
                n.DeletedBy == null &&
                rolesId.Any(r => r == n.RefUserAccessNavigation.Role.Id) ).ToList();
            return Task.FromResult(userRoles);
        }
    }
}
