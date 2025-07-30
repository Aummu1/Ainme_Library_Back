using AnimeApi.Data;
using AnimeApi.Models;
using BCrypt.Net;
using Microsoft.EntityFrameworkCore; // ✅ สำคัญสำหรับ EF async methods
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Configuration; // สำหรับ IConfiguration

namespace AnimeApi.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public UserService(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<User> RegisterAsync(User user)
        {
            // ✅ ตรวจ email หรือ username ซ้ำ
            var existingUser = await _context.User
                .FirstOrDefaultAsync(u => u.Email == user.Email || u.Username == user.Username);

            if (existingUser != null)
            {
                throw new Exception("Email or Username already exists.");
            }

            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            user.CreatedAt = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");

            _context.User.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User?> LoginAsync(string usernameOrEmail, string password)
        {
            var user = await _context.User
                .FirstOrDefaultAsync(u => u.Username == usernameOrEmail || u.Email == usernameOrEmail);

            if (user == null)
                return null;

            // ✅ ตรวจรหัสผ่าน
            bool isValid = BCrypt.Net.BCrypt.Verify(password, user.Password);
            return isValid ? user : null;
        }

        public async Task UpdateLastLoginAsync(User user)
        {
            var thaiTime = TimeZoneInfo.ConvertTimeBySystemTimeZoneId(DateTime.UtcNow, "SE Asia Standard Time");
            user.LastLogin = thaiTime.ToString("yyyy-MM-dd HH:mm:ss");

            _context.User.Update(user);
            await _context.SaveChangesAsync();
        }
    }
}
