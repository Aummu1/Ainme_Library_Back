using AnimeApi.Models;
using Microsoft.EntityFrameworkCore; // ✅ ต้องมีอันนี้

namespace AnimeApi.Services
{
    public interface IUserService
    {
        Task<User> RegisterAsync(User user);
        Task<User?> LoginAsync(string usernameOrEmail, string password);
        Task UpdateLastLoginAsync(User user);
        DbSet<User> Users { get; }
        Task<int> SaveChangesAsync();
    }
}
