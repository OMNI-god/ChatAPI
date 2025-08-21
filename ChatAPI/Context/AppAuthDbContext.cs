using ChatAPI.Model.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace ChatAPI.Context
{
    public class AppAuthDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
    {
        private readonly IConfiguration configuration;

        public AppAuthDbContext(DbContextOptions<AppAuthDbContext> options, IConfiguration configuration) : base(options)
        {
            this.configuration = configuration;
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            //psql
            //user to refresh token one to many relationship
            modelBuilder.Entity<RefreshToken>()
                .HasOne(rt => rt.User)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(rt => rt.UserId);
            modelBuilder.Entity<RefreshToken>()
                .Property(rt => rt.Created)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
            modelBuilder.Entity<RefreshToken>()
                .Property(rt => rt.Expires)
                .HasDefaultValueSql($"CURRENT_TIMESTAMP + interval '{configuration.GetValue<string>("JWT:RefreshTokenExpirationInDays")} days'");

            //add indexing to refresh token
            modelBuilder.Entity<RefreshToken>()
                .HasIndex(rt => rt.UserId);
            modelBuilder.Entity<RefreshToken>()
                .HasIndex(rt => rt.TokenHash);
            modelBuilder.Entity<RefreshToken>()
                .HasIndex(rt => new { rt.UserId, rt.TokenHash });

            //add indexing to user
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();
            modelBuilder.Entity<User>()
                .HasIndex(u => u.UserName)
                .IsUnique();

            //user to message one to many relationship
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

            //add indexing to message
            modelBuilder.Entity<Message>()
                .HasIndex(message => message.SenderId);
            modelBuilder.Entity<Message>()
                .HasIndex(message => message.ReceiverId);
            modelBuilder.Entity<Message>()
                .HasIndex(message => message.SentAt);
            modelBuilder.Entity<Message>()
                .HasIndex(message => new { message.SenderId, message.ReceiverId });

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

            var roles = new List<IdentityRole<Guid>>
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
