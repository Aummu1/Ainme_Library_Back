using AnimeApi.Models;
using AnimeApi.Services;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace AnimeApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;

        public UserController(IUserService userService, IConfiguration configuration)
        {
            _userService = userService;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] User user)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdUser = await _userService.RegisterAsync(user);
            return Ok(createdUser);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userService.LoginAsync(dto.UsernameOrEmail, dto.Password);
            if (user == null)
                return Unauthorized(new { message = "Invalid username/email or password" });

            user.LastLogin = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");
            await _userService.UpdateLastLoginAsync(user);

            var token = GenerateJwtToken(user);

            return Ok(new
            {
                token,
                user = new
                {
                    user.UserId,
                    user.Username,
                    user.Email,
                    user.LastLogin
                }
            });
        }

        // ✅ ฟังก์ชันสร้าง JWT ที่ย้ายมาไว้ใน Controller
        private string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                _configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: null,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(3),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserDto dto)
        {
            var user = await _userService.Users.FindAsync(id);
            if (user == null)
                return NotFound("User not found");

            if (!string.IsNullOrWhiteSpace(dto.Username))
                user.Username = dto.Username;

            if (!string.IsNullOrWhiteSpace(dto.ImageBase64))
            {
                try
                {
                    user.ProfileImage = Convert.FromBase64String(
                        dto.ImageBase64.Contains(",")
                            ? dto.ImageBase64.Split(",")[1]
                            : dto.ImageBase64
                    );
                }
                catch
                {
                    return BadRequest("Invalid base64 image string");
                }
            }

            await _userService.SaveChangesAsync();
            return Ok("User updated successfully");
        }
    }
}
