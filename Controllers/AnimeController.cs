using AnimeApi.Models;
using AnimeApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace AnimeApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AnimeController : ControllerBase
    {
        private readonly IAnimeService _animeService;

        public AnimeController(IAnimeService animeService)
        {
            _animeService = animeService;
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetByUserId(int userId)
        {
            var animes = await _animeService.GetByUserIdAsync(userId);

            var result = animes.Select(a => new AnimeResponseDto
            {
                Id = a.Id,
                AnimeName = a.AnimeName,
                Description = a.Description,
                Category = a.Category,
                Status = a.Status,
                ImageBase64 = a.Image != null ? Convert.ToBase64String(a.Image) : null,
                UserId = a.UserId
            });

            return Ok(result);
        }

        public class AnimeResponseDto
        {
            public int Id { get; set; }
            public string AnimeName { get; set; }
            public string Description { get; set; }
            public string Category { get; set; }
            public string Status { get; set; }
            public string? ImageBase64 { get; set; }  // ✅ อนุญาตให้ null ได้
            public int UserId { get; set; }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var anime = await _animeService.GetByIdAsync(id);
            if (anime == null)
            {
                return NotFound();
            }

            var result = new AnimeResponseDto
            {
                Id = anime.Id,
                AnimeName = anime.AnimeName,
                Description = anime.Description,
                Category = anime.Category,
                Status = anime.Status,
                ImageBase64 = anime.Image != null ? Convert.ToBase64String(anime.Image) : null
            };

            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromForm] AnimeDto dto)
        {
            byte[] imageBytes;

            using (var memoryStream = new MemoryStream())
            {
                await dto.Image.CopyToAsync(memoryStream);
                imageBytes = memoryStream.ToArray();
            }

            var anime = new AnimeInfo
            {
                AnimeName = dto.AnimeName,
                Description = dto.Description,
                Category = dto.Category,
                Status = dto.Status,
                Image = imageBytes, // เก็บเป็น blob
                UserId = dto.UserId
            };

            await _animeService.AddAsync(anime);

            return Ok(anime); // แค่ตอบกลับเฉย ๆ ไม่ต้องบอกว่าไปดูที่ไหน
        }

        public class AnimeDto
        {
            public string AnimeName { get; set; }
            public string Description { get; set; }
            public string Category { get; set; }
            public string Status { get; set; }
            public IFormFile? Image { get; set; }
            public int UserId { get; set; }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromForm] AnimeDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // 🔍 โหลดข้อมูลเดิมจาก DB
            var existing = await _animeService.GetByIdAsync(id);
            if (existing == null)
                return NotFound();

            // ✅ ใช้ของใหม่ถ้ามี, ถ้าไม่มีให้ใช้ของเดิม
            var updatedAnime = new AnimeInfo
            {
                Id = id,
                AnimeName = string.IsNullOrWhiteSpace(dto.AnimeName) ? existing.AnimeName : dto.AnimeName,
                Description = string.IsNullOrWhiteSpace(dto.Description) ? existing.Description : dto.Description,
                Category = string.IsNullOrWhiteSpace(dto.Category) ? existing.Category : dto.Category,
                Status = string.IsNullOrWhiteSpace(dto.Status) ? existing.Status : dto.Status,
                Image = existing.Image // default ให้ใช้ของเดิม
            };

            // ✅ ถ้ามี image ใหม่แนบมา ค่อยอัปเดต
            if (dto.Image != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await dto.Image.CopyToAsync(memoryStream);
                    updatedAnime.Image = memoryStream.ToArray();
                }
            }

            var updated = await _animeService.UpdateAsync(id, updatedAnime);
            return updated == null ? NotFound() : Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _animeService.DeleteAsync(id);
            return deleted ? NoContent() : NotFound();
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return BadRequest("Query is required.");

            var results = await _animeService.SearchAnimeAsync(query);
            return Ok(results);
        }
    }
}
