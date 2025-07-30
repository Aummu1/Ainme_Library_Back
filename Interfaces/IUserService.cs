using AnimeApi.Models;

namespace AnimeApi.Services
{
    public interface IUserService
    {
        Task<User> RegisterAsync(User user);
        Task<User?> LoginAsync(string usernameOrEmail, string password);
        Task UpdateLastLoginAsync(User user);
    }
}
