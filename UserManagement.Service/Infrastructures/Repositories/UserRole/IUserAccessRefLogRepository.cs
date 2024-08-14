using Microsoft.AspNetCore.Mvc.RazorPages;
using Shared.Infrastructures.Repositories.EntityFramework;
using Shared.Models.Core;
using Shared.Models.Filters;
using User_Management.Models.Entities;
using User_Management.Models.Filters;

namespace User_Management.Infrastructures.Repositories.RoleUser
{
    public class EFTemporaryRoleUserRefRepository : EFBaseRepository<TmpRoleUserRef, string>, ITemporaryRoleUserRefRepository
    {
        public EFTemporaryRoleUserRefRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            IsAuditEntity = false;
            NoTraciking = true;
        }
        public override Task<TmpRoleUserRef> AddAsync(TmpRoleUserRef input)
        {
            if (string.IsNullOrEmpty(input.Id))
                input.Id = Guid.NewGuid().ToString();
            return base.AddAsync(input);
        }
        public override Task<TmpRoleUserRef> GetById(string id, bool isDeleted = false)
        {
            NoTraciking = true;
            return Task.FromResult(GetAll().FirstOrDefault(n => n.Id == id));
        }

        public Task<Paged<TmpRoleUserRef>> GetPaged(string uniqueId, PageFilter page, RoleUserFilter filter)
        {
            filter.Name = (filter.Name ?? String.Empty).ToLower();
            return base.GetPaged(page, n => n.ParentId == uniqueId
            && (n.Username.ToLower().Contains(filter.Name)
             || n.Email.ToLower().Contains(filter.Name))
            );
        }

        public async Task<bool> IsDuplicate(string uniqueId, string userName)
        {
            return (await base.GetPaged(new PageFilter()
            {
                Page = 1,
                Size = 1
            }, n => n.ParentId == uniqueId
            && (n.Username == userName))).TotalItems > 0;
        }
    }

}
