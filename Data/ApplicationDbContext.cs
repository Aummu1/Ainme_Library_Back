using Microsoft.EntityFrameworkCore;
using AnimeApi.Models;

namespace AnimeApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<AnimeInfo> AnimeInfo { get; set; }
        public DbSet<User> User { get; set; }
    }
}
