using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SignalR_Test.Models;

namespace ChatAPI.Context
{
    public class AppAuthDbContext:IdentityDbContext
    {
        public AppAuthDbContext(DbContextOptions<AppAuthDbContext> options): base(options)
        {
            
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            var userRoleID = "881c2c92-ec07-41f0-bbd3-63dde7c1734d";
            var adminRoleID = "9b428886-5fef-4010-a549-7048e184df74";

            var roles=new List<IdentityRole>
            {
                new IdentityRole
                {
                    Id = userRoleID,
                    ConcurrencyStamp = userRoleID,
                    Name = "User",
                    NormalizedName = "USER"
                },
                new IdentityRole
                {
                    Id = adminRoleID,
                    ConcurrencyStamp = adminRoleID,
                    Name = "Admin",
                    NormalizedName = "ADMIN"
                }
            };
            modelBuilder.Entity<IdentityRole>().HasData(roles);
        }
        public DbSet<User> Users { get; set; }
    }
}
