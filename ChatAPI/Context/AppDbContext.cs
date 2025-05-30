using ChatAPI.Model;
using Microsoft.EntityFrameworkCore;
using SignalR_Test.Models;

namespace SignalR_Test.Contexts
{
    public class AppDbContext:DbContext
    {
        private readonly IConfiguration configuration;
        public AppDbContext(DbContextOptions<AppDbContext> options,IConfiguration configuration) :base(options)
        {
            this.configuration = configuration;
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            ////psql
            //modelBuilder.Entity<RefreshToken>()
            //    .HasOne(rt => rt.User)
            //    .WithMany(u => u.RefreshTokens)
            //    .HasForeignKey(rt => rt.UserId);
            //modelBuilder.Entity<RefreshToken>()
            //    .Property(rt => rt.Created)
            //    .HasDefaultValueSql("GETUTCDATE()");
            //modelBuilder.Entity<RefreshToken>()
            //    .Property(rt => rt.Expires)
            //    .HasDefaultValueSql($"DATEADD(DAY, {configuration.GetValue<string>("jwt:RefreshTokenExpirationInDays")}, GETUTCDATE())");
            //mssql
            modelBuilder.Entity<RefreshToken>()
                .HasOne(rt => rt.User)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(rt => rt.UserId);
            modelBuilder.Entity<RefreshToken>()
                .Property(rt => rt.Created)
                .HasDefaultValue(DateTime.UtcNow);
            modelBuilder.Entity<RefreshToken>()
                .Property(rt => rt.Expires)
                .HasDefaultValue(DateTime.UtcNow.AddDays(Convert.ToInt32(configuration["jwt:RefreshTokenExpirationInDays"])));
        }
        public DbSet<RefreshToken> RefreshTokens { get; set; }
        public DbSet<ChatHistory> ChatHistory { get; set; }
    }
}
