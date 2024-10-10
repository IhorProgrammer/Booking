using BookingLibrary.Data.DAO;
using Microsoft.EntityFrameworkCore;

namespace Email.API.Data
{
    public class EmailDBContext : DbContext
    {
        public EmailDBContext( DbContextOptions<EmailDBContext> options ) : base(options) { }

        public DbSet<TokenInfoDAO> TokenInfoData => Set<TokenInfoDAO>();
        public DbSet<UserInfoDAO> UserInfoData => Set<UserInfoDAO>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TokenInfoDAO>()
                     .HasOne(t => t.UserInfoData)
                     .WithMany(u => u.Tokens)
                     .HasForeignKey(t => t.UserID)
                     .OnDelete(DeleteBehavior.Cascade);
            base.OnModelCreating(modelBuilder);
        }
    }
}
