using AnimeApi.Data;
using AnimeApi.Models;
using Microsoft.EntityFrameworkCore;

namespace AnimeApi.Services
{
    public class AnimeService : IAnimeService
    {
        private readonly AppDbContext _context;

        public AnimeService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<AnimeInfo>> GetAllAsync()
        {
            return await _context.AnimeInfo.ToListAsync();
        }

        public async Task<AnimeInfo?> GetByIdAsync(int id)
        {
            return await _context.AnimeInfo.FindAsync(id);
        }

        public async Task<AnimeInfo> AddAsync(AnimeInfo anime)
        {
            _context.AnimeInfo.Add(anime);
            await _context.SaveChangesAsync();
            return anime;
        }

        public async Task<AnimeInfo> UpdateAsync(int id, AnimeInfo anime)
        {
            var existing = await _context.AnimeInfo.FindAsync(id);
            if (existing == null) return null;

            existing.AnimeName = anime.AnimeName;
            existing.Description = anime.Description;
            existing.Category = anime.Category;
            existing.Status = anime.Status;
            existing.Image = anime.Image;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var anime = await _context.AnimeInfo.FindAsync(id);
            if (anime == null) return false;

            _context.AnimeInfo.Remove(anime);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<AnimeInfo>> GetByUserIdAsync(int userId)
        {
            return await _context.AnimeInfo
                .Where(a => a.UserId == userId)
                .ToListAsync();
        }

        public async Task<IEnumerable<AnimeInfo>> SearchAnimeAsync(string query)
        {
            return await _context.AnimeInfo
                .Where(a => a.AnimeName.ToLower().Contains(query.ToLower()))
                .ToListAsync();
        }

        public async Task<AnimeInfo?> ToggleFavoriteAsync(int id)
        {
            var anime = await _context.AnimeInfo.FindAsync(id);
            if (anime == null) return null;

            anime.Favorite = !anime.Favorite; // toggle
            await _context.SaveChangesAsync();
            return anime;
        }
    }
}
