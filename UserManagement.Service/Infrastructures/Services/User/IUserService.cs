using Shared.Models;
using Shared.Models.Core;
using User_Management.Models.Dtos.User;
using static User_Management.Constants.UserConstants;

namespace User_Management.Infrastructures.Services
{
    public interface IUserService
    {
        Task<Result<LoginStateEnum, User?>> LoginAsync(string username, string password);
        Task<Result<SearchStateEnum, List<User>>> Search(string key);
        Task<Result<AccessCotntrolListState, List<MdRoleAccessRefDto>?>> GetAccessControl();
        
    }
}