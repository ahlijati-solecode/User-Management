using Shared.Infrastructures.Repositories;
using User_Management.Models.Entities;

namespace User_Management.Infrastructures.Repositories.RoleUser
{
    public interface ITemporaryRoleUserRepository : IBaseRepository<TmpRoleUser, string>
    {
        Task CleanUp();
    }

}
