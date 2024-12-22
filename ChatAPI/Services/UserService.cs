using ChatAPI.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SignalR_Test.Contexts;
using SignalR_Test.Models;

namespace ChatAPI.Services
{
    public class UserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User> GetUserByEmailAsync(string username,string password)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username&&u.PasswordHash==password);
        }

        public async Task<OperationResult> CreateUserAsync(dynamic data)
        {
            if (IsRegistered((string)data.username))
            {
                return new OperationResult
                {
                    Success = false,
                    Message = "Username already taken"
                };
            }

            User user = new User
            {
                Id = Guid.NewGuid(),
                Username = (string)data.username,
                PasswordHash = (string)data.password,
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new OperationResult
            {
                Success = true,
                Message = "User created successfully",
            };
        }
        private bool IsRegistered(string username)
        {
            return _context.Users.Any(user => user.Username == username);
        }
    }
}
