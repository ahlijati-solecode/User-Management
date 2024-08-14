using System.Data;
using User_Management.Models.Entities;

namespace User_Management.Infrastructures.Repositories
{
    public interface IRefMenuRepository
    {
        Task<List<MdRefMenu>> GetAllMenuData(string sort = "id asc");
        Task<MdRefMenu> GetById(int menuId, bool isDeleted = false);
        Task<MdRefMenu> GetByName(string name);
        Task<MdRefMenu> AddAsync(MdRefMenu input);
    }
}
