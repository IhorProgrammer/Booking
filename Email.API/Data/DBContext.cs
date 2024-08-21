using Microsoft.EntityFrameworkCore;
using Email.API.Data.Entities;

namespace Email.API.Data
{
    public class DBContext : DbContext
    {
        public DBContext( DbContextOptions<DBContext> options ) : base(options) { }

        public DbSet<TokenInfoData> TokenInfoData => Set<TokenInfoData>();
        public DbSet<UserInfoData> UserInfoData => Set<UserInfoData>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TokenInfoData>()
                     .HasOne(t => t.UserInfoData)
                     .WithMany(u => u.Tokens)
                     .HasForeignKey(t => t.UserID)
                     .OnDelete(DeleteBehavior.Cascade);
            base.OnModelCreating(modelBuilder);
        }
    }
}
