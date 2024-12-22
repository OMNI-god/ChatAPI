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

        public async Task CreateUserAsync(dynamic Data)
        {
            User user = new User
            {
                Id = Guid.NewGuid(),
                Username = (string)Data.username,
                PasswordHash = (string)Data.password,
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }
    }
}
