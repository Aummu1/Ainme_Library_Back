using AnimeApi.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AnimeApi.Services
{
    public interface IAnimeService
    {
        Task<IEnumerable<AnimeInfo>> GetAllAsync();
        Task<AnimeInfo?> GetByIdAsync(int id);
        Task<AnimeInfo> AddAsync(AnimeInfo anime);
        Task<AnimeInfo> UpdateAsync(int id, AnimeInfo anime);
        Task<bool> DeleteAsync(int id);
    }
}
