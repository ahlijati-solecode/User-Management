using Microsoft.EntityFrameworkCore;
using Shared.Helpers.HelperEnums;
using Shared.Helpers.HelperModels;
using Shared.Infrastructures.Repositories.EntityFramework;
using User_Management.Constants;
using User_Management.Models.Entities;
using User_Management.Models.Entities.Custom.RefUserAccess;
using User_Management.Models.Filters;
using User_Management.Queries;

namespace User_Management.Infrastructures.Repositories.UserAccess
{
    public class EFUserAccessRepository : EFPagedBaseRepository<MdRoleAccess, RoleAccessFilter, MdUserAccessPaged>, IRoleAccessRepository
    {
        public EFUserAccessRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public override string SelectPagedQuery => UserAccessQueries.SelectPagedQuery;

        public override string TableName => RoleAccessConstant.TableName;

        public override string TableLogName => RoleAccessConstant.TableLogName;

        public override string ReferenceField => RoleAccessConstant.ReferenceField;

        public override string JoinQuery => UserAccessQueries.JoinQuery;
        public override Task<MdRoleAccess> GetById(int id, bool isDeleted = false)
        {
            NoTraciking = true;
            Includes($"{nameof(MdRoleAccess.MdUserAccessRefs)}.{nameof(MdRoleAccessRef.LgUserAccessRefs)}");
            MdRoleAccess item;
            if (isDeleted)
                item = GetAll().FirstOrDefault(n => n.Id == id);
            else
                item = GetAll().FirstOrDefault(n => n.Id == id && n.DeletedBy == null);
            return Task.FromResult(item);
        }
        public Task<MdRoleAccess> GetPreviewByIdAsync(int id)
        {
            //Includes($"{nameof(MdRoleAccess.MdUserAccessRefs)}.{nameof(MdRoleAccessRef.LgUserAccessRefs)}",
            //    $"{nameof(MdRoleAccess.LgUserAccessRefs)}.{nameof(LgRoleAccessRef.RefMenu)}",
            //    $"{nameof(MdRoleAccess.LgUserAccesses)}.{nameof(LgRoleAccess.Role)}");
            NoTraciking = true;
            var item = GetAll().FirstOrDefault(n => n.Id == id && n.DeletedBy == null);
            if (item == null)
                return null;

            var latestUserAccessLog = _context.Set<LgRoleAccess>()
                .Include(n => n.Role)
                .Where(n => n.ParentId == item.Id).OrderBy(n => n.Id).LastOrDefault();

            if (latestUserAccessLog != null)
            {
                item.IsActive = latestUserAccessLog.IsActive;
                item.RoleId = latestUserAccessLog.RoleId;
                item.Role = latestUserAccessLog.Role;
            }
            item.MdUserAccessRefs = _context.Set<MdRoleAccessRef>()
                .Where(n => n.RefUserAccess == item.Id)
                .Include(n => n.LgUserAccessRefs)
                .ThenInclude(n => n.RefMenu)
                .ToList();
            if (item.MdUserAccessRefs != null)
            {
                PreviewAccessReference(item);
            }
            return Task.FromResult(item);
        }

        protected void PreviewAccessReference(MdRoleAccess? item)
        {
            var query = _context.Set<LgRoleAccessRef>()
                        .Include(n => n.RefMenu)
                        .Where(n => n.RefUserAccess == item.Id)
                        .OrderBy(n => n.Id).ToList();
            item.LgUserAccessRefs = query;
            if (item != null)
                foreach (var reference in item.MdUserAccessRefs)
                {
                    var latestUserAccessRefLog = query
                        .LastOrDefault(n => n.ParentId == reference.Id);
                    if (latestUserAccessRefLog != null)
                    {
                        reference.IsView = latestUserAccessRefLog.IsView;
                        reference.IsDelete = latestUserAccessRefLog.IsDelete;
                        reference.IsEdit = latestUserAccessRefLog.IsEdit;
                        reference.IsCreate = latestUserAccessRefLog.IsCreate;
                        reference.RefMenuId = latestUserAccessRefLog.RefMenuId;
                        reference.RefMenu = latestUserAccessRefLog.RefMenu;
                    }
                }
        }

        public override void BeforeRenderItems(IEnumerable<MdUserAccessPaged> items)
        {
            //Includes($"{nameof(MdRoleAccess.MdUserAccessRefs)}",
            //    $"{nameof(MdRoleAccess.LgUserAccessRefs)}");
            var ids = items.Select(n => n.Id).ToArray();
            //var existingDbs = GetAll().Where(n => ids.Contains(n.Id)).ToList();
            foreach (var item in items)
            {
                var entity = _context.Set<MdRoleAccess>()
                    .Include(n => n.MdUserAccessRefs)

                    .Include(n => n.LgUserAccessRefs).ThenInclude(n => n.RefMenu)
                    .FirstOrDefault(n => n.Id == item.Id);
                if (entity != null)
                {
                    item.MdUserAccessRefs = entity.MdUserAccessRefs;
                    item.LgUserAccessRefs = entity.LgUserAccessRefs;
                    PreviewAccessReference(item);
                }
            }
        }
        protected override List<FilterBuilderModel> BuildQuery(List<FilterBuilderModel> param, RoleAccessFilter filter)
        {
            BuildFilterRole(param, filter);
            BuildFilterStatus(param, filter);
            return param;
        }

        private void BuildFilterStatus(List<FilterBuilderModel> param, RoleAccessFilter filter)
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

        private void BuildFilterRole(List<FilterBuilderModel> param, RoleAccessFilter filter)
        {
            if (!string.IsNullOrEmpty(filter.Name))
            {
                param.Add(new FilterBuilderModel("name", FilterBuilderEnum.LIKE, $"'%{filter.Name}%'"));
            }

        }

        public Task<bool> IsDuplicateAsync(int roleId, int id)
        {
            return Task.FromResult(GetAll().Any(n => n.DeletedBy == null && n.RoleId == roleId && n.Id != id));
        }
    }
}
