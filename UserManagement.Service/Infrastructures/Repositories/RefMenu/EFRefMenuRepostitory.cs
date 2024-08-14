using Dapper;
using Shared.Infrastructures.Repositories.EntityFramework;
using User_Management.Models.Entities;
using User_Management.Queries;

namespace User_Management.Infrastructures.Repositories.RefMenu
{
    public class EFRefMenuRepostitory : EFBaseRepository<MdRefMenu>, IRefMenuRepository
    {
        public EFRefMenuRepostitory(IServiceProvider serviceProvider) : base(serviceProvider)
        {
        }

        public async Task<List<MdRefMenu>> GetAllMenuData(string sort = "id asc")
        {
            using (var connection = OpenConnection())
            {
                var metaData = new Dictionary<string, object>();
                metaData.Add("sort", sort);

                metaData.Add("select", LgRefMenuQueries.SelectPagedQuery);

                var querySQL = BuildQuery(LgRefMenuQueries.Query.Replace("@sort", "order by " + sort.ToString()), metaData);
                var items = (List<MdRefMenu>)await connection.QueryAsync<MdRefMenu>(querySQL);

                return items;
            }
        }

        public Task<MdRefMenu> GetByName(string name)
        {
            return Task.FromResult(GetAll().FirstOrDefault(n => n.Name.Equals(name)));
        }
    }
}
