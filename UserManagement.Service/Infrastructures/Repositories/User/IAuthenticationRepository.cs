using Shared.Models.Core;

namespace Shared.Infrastructures.Repositories
{
    public interface ISearchUserRepository: IUserRepository
    {
    }
    public interface IAuthenticationRepository
    {
        Task<User?> LoginAsync(string username, string password);
    }
}