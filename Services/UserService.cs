using messanger.Data;
using messanger.DTOs;
using messanger.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace messanger.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _context;
        public UserService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<string> RegisterAsync(RegisterDto dto)
        {
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (existingUser != null)
                return "User already exists";

            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                Password = HashPassword(dto.Password)
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return "registered successfully";
        }

        public async Task<string> LoginAsync(LoginDto dto)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (user == null) return "User not found";

            var hash = HashPassword(dto.Password);

            if (user.Password != hash) return "InvalidPassword";

            return "Login successful";
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(password);
            var hash = sha256.ComputeHash(bytes);

            return Convert.ToBase64String(hash);
        }
    }
}
