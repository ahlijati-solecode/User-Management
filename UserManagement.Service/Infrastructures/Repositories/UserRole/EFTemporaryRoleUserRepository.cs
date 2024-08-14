using Shared.Infrastructures.Repositories.EntityFramework;
using User_Management.Models.Entities;
using User_Management.Queries;

namespace User_Management.Infrastructures.Repositories.RoleUser
{
    public class EFTemporaryRoleUserRepository : EFBaseRepository<TmpRoleUser, string>, ITemporaryRoleUserRepository
    {
        public EFTemporaryRoleUserRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            NoTraciking = true;
        }
        public override Task<TmpRoleUser> GetById(string id, bool isDeleted = false)
        {
            Includes(nameof(TmpRoleUser.TmpRoleUserRefs), nameof(TmpRoleUser.Role));
            NoTraciking = true;
            var result = GetAll().FirstOrDefault(n => n.Id == id);
            return Task.FromResult(result);
        }
        public override Task<TmpRoleUser> AddAsync(TmpRoleUser input)
        {
            if (string.IsNullOrEmpty(input.Id))
                input.Id = Guid.NewGuid().ToString();
            return base.AddAsync(input);
        }

        public Task CleanUp()
        {
            using var connection = OpenConnection();
            var time = DateTime.Now.AddHours(-1).ToString("yyyy-MM-dd HH:mm");
            var command = connection.CreateCommand();
            command.CommandText = String.Format(UserRoleQueries.DeleteTemporaryRef, time);
            command.ExecuteNonQuery();
            command = connection.CreateCommand();
            command.CommandText = String.Format(UserRoleQueries.DeleteTemporary, time);
            command.ExecuteNonQuery();
            return Task.CompletedTask;
        }
    }

}
