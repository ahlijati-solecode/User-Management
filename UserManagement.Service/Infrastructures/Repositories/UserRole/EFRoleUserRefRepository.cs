using Microsoft.EntityFrameworkCore;
using Shared.Infrastructures.Repositories.EntityFramework;
using Shared.Models.Core;
using User_Management.Helpers;
using User_Management.Models.Entities;

namespace User_Management.Infrastructures.Repositories.RoleUser
{
    public class EFRoleUserRefRepository : EFBaseRepository<MdRoleUserRef>, IRoleUserRefRepository
    {
        public EFRoleUserRefRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
        public Task<List<MdRoleUserRef>> GetByRoles(int[] rolesId)
        {
            var userRoles = GetAll()
               .Include(n => n.Parent)
               .Where(n => n.Parent.DeletedBy == null && rolesId.Any(r => r == n.Parent.RoleId));
            return Task.FromResult(userRoles.ToList());
        }
        public Task<List<User>> GetAllApproverAsync()
        {
            var userName = _currentUserService.CurrentUser?.UserName ?? String.Empty;
            var userRoles = GetAll()
               .Where(n => n.Username != userName)
               .Include(n => n.Parent)
               .ThenInclude(n => n.Role)
               .Where(n => n.IsApprover == true && n.Parent.ApprovedBy != null && n.Parent.IsActive && n.Parent.DeletedBy == null)
               .OrderBy(n => n.Username)
               .ToList();
            var users = userRoles.Select(n => new User()
            {
                Email = n.Email,
                FullName = n.FullName,
                UserName = n.Username,
                Departement = n.Departement
            }).Distinct(new UsernameComparer()).ToList();
            return Task.FromResult(users);
        }

        public Task<List<MdRoleUserRef>> GetByUserName(string userName)
        {
            NoTraciking = true;
            var userRoles = GetAll()
                .Include(n => n.Parent)
                .ThenInclude(n => n.Role)
                .Where(n => n.Username.ToLower() == userName.ToLower() && n.Parent.ApprovedBy != null && n.Parent.DeletedBy == null).ToList();
            return Task.FromResult(userRoles);
        }
    }

}
