using ChatAPI.Model.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ChatAPI.Context
{
    public class AppAuthDbContext:IdentityDbContext<User,IdentityRole<Guid>,Guid>
    {
        private readonly IConfiguration configuration;

        public AppAuthDbContext(DbContextOptions<AppAuthDbContext> options,IConfiguration configuration): base(options)
        {
            this.configuration = configuration;
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //psql
            modelBuilder.Entity<RefreshToken>()
                .HasOne(rt => rt.User)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(rt => rt.UserId);
            modelBuilder.Entity<RefreshToken>()
                .Property(rt => rt.Created)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
            modelBuilder.Entity<RefreshToken>()
            .Property(rt => rt.Expires)
                .HasDefaultValueSql($"CURRENT_TIMESTAMP + interval '{configuration.GetValue<string>("jwt:RefreshTokenExpirationInDays")} days'");
            modelBuilder.Entity<Message>()
                .HasOne(m => m.Sender)
                .WithMany(u => u.SentMessages)
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Message>()
                .HasOne(m => m.Receiver)
                .WithMany(u => u.ReceivedMessages)
                .HasForeignKey(m => m.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Message>()
                .Property(m => m.SentAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
            ////mssql
            //modelBuilder.Entity<RefreshToken>()
            //    .HasOne(rt => rt.User)
            //    .WithMany(u => u.RefreshTokens)
            //    .HasForeignKey(rt => rt.UserId);
            //modelBuilder.Entity<RefreshToken>()
            //    .Property(rt => rt.Created)
            //    .HasDefaultValue(DateTime.UtcNow);
            //modelBuilder.Entity<RefreshToken>()
            //    .Property(rt => rt.Expires)
            //    .HasDefaultValue(DateTime.UtcNow.AddDays(Convert.ToInt32(configuration["jwt:RefreshTokenExpirationInDays"])));
            var userRoleID = Guid.Parse("881c2c92-ec07-41f0-bbd3-63dde7c1734d");
            var adminRoleID = Guid.Parse("9b428886-5fef-4010-a549-7048e184df74");

            var roles=new List<IdentityRole<Guid>>
            {
                new IdentityRole<Guid>
                {
                    Id = userRoleID,
                    ConcurrencyStamp = userRoleID.ToString(),
                    Name = "User",
                    NormalizedName = "USER"
                },
                new IdentityRole < Guid >
                {
                    Id = adminRoleID,
                    ConcurrencyStamp = adminRoleID.ToString(),
                    Name = "Admin",
                    NormalizedName = "ADMIN"
                }
            };
            modelBuilder.Entity<IdentityRole<Guid>>().HasData(roles);
        }
        public DbSet<User> Users { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
    }
}
