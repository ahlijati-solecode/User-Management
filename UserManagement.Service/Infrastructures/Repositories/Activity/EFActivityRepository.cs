using Shared.Infrastructures.Repositories;
using Shared.Infrastructures.Repositories.EntityFramework;
using Shared.Models.Core;
using Shared.Models.Entities;
using Shared.Models.Filters;
using User_Management.Models.Entities;

namespace User_Management.Infrastructures.Repositories
{
    public class EFActivityRepository : EFBaseRepository<LgActivity>, IActivityRepository
    {
        public EFActivityRepository(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            IsAuditEntity = false;
        }

        public Task<LgActivity> AddAsync(string note, string userName = "")
        {
            return AddAsync(new LgActivity()
            {
                Activity = note,
                Time = DateTime.Now,
                Username = string.IsNullOrEmpty(userName) ? _currentUserService.CurrentUser?.UserName : userName
            });
        }

        public Task<Paged<LgActivity>> GetPaged(PageFilter page, ActivityFilter filter)
        {
            var predicate = LinqExtensions.True<LgActivity>();
            if (!string.IsNullOrEmpty(filter.UserName))
            {
                var name = filter.UserName?.ToLower();
                predicate = predicate.And(n => n.Username.ToLower().Contains(name));
            }
            if (filter.StartDate != null)
            {
                var date = filter.StartDate.Value.Date;
                predicate = predicate.And(n => n.Time >= date);
            }
            if (filter.EndDate != null)
            {
                var endDate = filter.EndDate.Value.Date;
                endDate = endDate.AddDays(1).AddSeconds(-1);
                predicate = predicate.And(n => n.Time <= endDate);
            }
            return base.GetPaged(page, predicate);
        }
    }
}