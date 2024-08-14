using Dapper;
using Microsoft.EntityFrameworkCore;
using Shared.Helpers;
using Shared.Helpers.HelperEnums;
using Shared.Helpers.HelperModels;
using Shared.Infrastructures.Repositories.EntityFramework;
using Shared.Models.Core;
using Shared.Models.Filters;
using User_Management.Constants;
using User_Management.Models.Entities;
using User_Management.Models.Entities.Custom.Role;
using User_Management.Models.Filters;
using User_Management.Queries;
using static Shared.Constants.ApiConstants;

namespace User_Management.Infrastructures.Repositories.Role
{
    public class EFRoleRepostitory : EFPagedBaseRepository<MdRole, RoleFilter, LgRolePaged>, IRoleRepository
    {
        public override string SelectPagedQuery => LgRoleQueries.SelectPagedQuery;

        public override string TableName => RoleConstants.TableName;

        public override string TableLogName => RoleConstants.TableLogName;

        public override string ReferenceField => RoleConstants.ReferenceField;
        public override string JoinQuery => LgRoleQueries.JoinQuery;

        public EFRoleRepostitory(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        protected override IQueryable<MdRole> BeforeGetById(IQueryable<MdRole> query)
        {
            return query.Include(n => n.MdUserAccesses).ThenInclude(n => n.MdUserAccessRefs);
        }
        public override Task<MdRole> GetById(int id, bool isDeleted = false)
        {
            return base.GetById(id, isDeleted);
        }
        #region Build Filter

        private static void BuildFilterName(RoleFilter filter, List<FilterBuilderModel> param)
        {
            if (!string.IsNullOrEmpty(filter.Name))
            {
                param.Add(new FilterBuilderModel("log.name", FilterBuilderEnum.LIKE, $"'%{filter.Name}%'"));
            }
        }

        private static void BuildFilterStatus(RoleFilter filter, List<FilterBuilderModel> param)
        {
            if (!string.IsNullOrEmpty(filter.Status) && Enum.TryParse<RoleConstants.StatusEnum>(filter.Status, true, out RoleConstants.StatusEnum status))
            {
                switch (status)
                {
                    case RoleConstants.StatusEnum.InActive:
                        param.Add(new FilterBuilderModel(@"log.is_active", FilterBuilderEnum.EQUALS, "false"));
                        break;

                    case RoleConstants.StatusEnum.Active:
                        param.Add(new FilterBuilderModel(@"log.is_active", FilterBuilderEnum.EQUALS, "true"));
                        break;

                    default:
                        break;
                }
            }
        }

        #endregion Build Filter

        public override MdRole MigrateData(MdRole exitingEntity, MdRole newEntity)
        {
            exitingEntity.Description = newEntity.Description;
            exitingEntity.IsActive = newEntity.IsActive;
            exitingEntity.IsAdmin = newEntity.IsAdmin;
            exitingEntity.Name = newEntity.Name;
            exitingEntity.DeletedBy = newEntity.DeletedBy;
            exitingEntity.DeletedDate = newEntity.DeletedDate;
            exitingEntity.ApprovedBy = newEntity.ApprovedBy;
            exitingEntity.ApprovedTime = newEntity.ApprovedTime;
            exitingEntity.ApprovalStatus = newEntity.ApprovalStatus;
            return exitingEntity;
        }

        protected override List<FilterBuilderModel> BuildQuery(List<FilterBuilderModel> param, RoleFilter filter)
        {
            BuildFilterStatus(filter, param);
            BuildFilterName(filter, param);
            return param;
        }

        public async Task<Paged<LgRolePaged>> GetPagedActive(RoleFilter roleFilter, PageFilter pageFilter)
        {
            var name = roleFilter?.Name?.ToLower() ?? String.Empty;
            var result = await base.GetPaged(pageFilter, n => n.Name.ToLower().Contains(name) && n.DeletedBy == null && n.ApprovedBy != null && n.IsActive);
            return new Paged<LgRolePaged>()
            {
                TotalItems = result.TotalItems,
                Items = result.Items.Select(n => new LgRolePaged()
                {
                    Name = n.Name,
                    Is_Active = n.IsActive,
                    Is_Admin = n.IsAdmin,
                    Id = n.Id,
                })
            };
        }

        public Task<MdRole> GetActiveById(int id)
        {
            var item = GetAll().FirstOrDefault(n => n.DeletedBy == null && n.ApprovedBy != null && n.IsActive && n.Id == id);
            return Task.FromResult(item);
        }

        public Task<bool> IsDuplicate(string name, int id)
        {
            var data = GetAll().Any(n => n.Name == name && id != n.Id && n.DeletedBy == null);
            return Task.FromResult(data);
        }

        public Task<MdRole> GetActiveByNameAsync(string roleName)
        {
            var data = GetAll().FirstOrDefault(n => n.Name == roleName && n.DeletedBy == null);
            return Task.FromResult(data);
        }
    }
}