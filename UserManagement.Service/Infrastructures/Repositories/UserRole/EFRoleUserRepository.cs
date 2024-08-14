using Shared.Helpers.HelperEnums;
using Shared.Helpers.HelperModels;
using Shared.Infrastructures.Repositories.EntityFramework;
using User_Management.Models.Entities;
using User_Management.Models.Filters;
using User_Management.Models.Entities.Custom.RoleUser;
using User_Management.Infrastructures.Repositories.RoleUser;
using User_Management.Queries;
using User_Management.Constants;
using Shared.Models.Core;

namespace User_Management.Infrastructures.Repositories.RoleUser
{
    public class EFRoleUserRepository : EFPagedBaseRepository<MdRoleUser, RoleUserFilter, RoleUserPaged>, IRoleUserRepository
    {
        public EFRoleUserRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }
        public Task<bool> IsDuplicate(int roleId, int id)
        {
            var data = GetAll().Any(n => n.RoleId == roleId && id != n.Id && n.DeletedBy == null);
            return Task.FromResult(data);
        }
        public override string SelectPagedQuery => RoleUserQueries.SelectPagedQuery;

        public override string TableName => RoleUserConstants.TableName;

        public override string TableLogName => RoleUserConstants.TableLogName;

        public override string ReferenceField => RoleUserConstants.ReferenceField;
        public override string JoinQuery => RoleUserQueries.JoinQuery;

        protected override List<FilterBuilderModel> BuildQuery(List<FilterBuilderModel> param, RoleUserFilter filter)
        {
            BuildFilterRole(param, filter);
            BuildFilterStatus(param, filter);
            return param;
        }

        public override Task<MdRoleUser> GetById(int id, bool isDeleted = false)
        {
            Includes(nameof(MdRoleUser.MdRoleUserRefs));
            NoTraciking = true;
            return base.GetById(id);
        }
        private void BuildFilterRole(List<FilterBuilderModel> param, RoleUserFilter filter)
        {
            if (!string.IsNullOrEmpty(filter.Name))
            {
                param.Add(new FilterBuilderModel("role.name", FilterBuilderEnum.LIKE, $"'%{filter.Name}%'"));
            }

        }
        private void BuildFilterStatus(List<FilterBuilderModel> param, RoleUserFilter filter)
        {
            if (!string.IsNullOrEmpty(filter.Status))
            {
                param.Add(new FilterBuilderModel("log.is_active", FilterBuilderEnum.EQUALS, filter.Status.Equals("1") ? "true" : "false"));
            }

        }

        public Task<MdRoleUser> GetUserRoleByName(string roleName)
        {
            var data = GetAll().FirstOrDefault(n => n.Role.Name == roleName && n.DeletedBy == null);
            return Task.FromResult(data);
        }
    }

}
