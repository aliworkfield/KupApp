using Microsoft.EntityFrameworkCore;
using CouponApp.Data;
using CouponApp.Models;
using CouponApp.DTOs;

namespace CouponApp.Services
{
    public class UserService
    {
        private readonly CouponAppContext _context;
        private readonly AuthService _authService;

        public UserService(CouponAppContext context, AuthService authService)
        {
            _context = context;
            _authService = authService;
        }

        public async Task<User?> AuthenticateUser(string username, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user != null && _authService.VerifyPassword(password, user.PasswordHash))
            {
                return user;
            }
            return null;
        }

        public async Task<User?> GetUserById(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<User?> GetUserByUsername(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<IEnumerable<User>> GetUsers(int skip = 0, int limit = 100)
        {
            return await _context.Users.Skip(skip).Take(limit).ToListAsync();
        }

        public async Task<User> CreateUser(UserCreateDto userDto)
        {
            var user = new User
            {
                Username = userDto.Username,
                Email = userDto.Email,
                PasswordHash = _authService.HashPassword(userDto.Password),
                Role = Enum.TryParse<UserRole>(userDto.Role, true, out var role) ? role : UserRole.User
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User?> UpdateUser(int id, UserUpdateDto userDto)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return null;

            if (userDto.Email != null)
            {
                user.Email = userDto.Email;
            }

            if (userDto.Role != null && Enum.TryParse<UserRole>(userDto.Role, true, out var role))
            {
                user.Role = role;
            }

            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<bool> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return false;

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UserExists(int id)
        {
            return await _context.Users.AnyAsync(e => e.Id == id);
        }
    }
}