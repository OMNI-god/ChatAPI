using ChatAPI.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SignalR_Test.Contexts;
using SignalR_Test.Models;
using System.Net;

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
            try
            {
                if (IsRegistered((string)data.username))
                {
                    return new OperationResult
                    {
                        HTTPCode = HttpStatusCode.Conflict,
                        Message = "User already exists",
                    };
                }

                User user = new User
                {
                    Id = Guid.NewGuid(),
                    Username = (string)data.username,
                    Email="test@mail.com",
                    PasswordHash = (string)data.password,
                    ProfilePicture=""
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return new OperationResult
                {
                    HTTPCode = HttpStatusCode.Created,
                    Message = "User created successfully",
                    Payload = new { username = user.Username, email = user.Email }
                };
            }
            catch (Exception ex)
            {
                return new OperationResult
                {
                    HTTPCode = HttpStatusCode.BadRequest,
                    Message = $"{ex.Message}\n{ex.InnerException}"
                };
            }
        }
        private bool IsRegistered(string username)
        {
            return _context.Users.Any(user => user.Username == username);
        }
    }
}
