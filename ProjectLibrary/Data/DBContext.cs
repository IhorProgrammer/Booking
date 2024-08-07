using ProjectLibrary.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace ProjectLibrary.Data
{
    public class DBContext : DbContext
    {
        public DBContext( DbContextOptions<DBContext> options ) : base(options) { }

        public DbSet<ClientData> ClientData => Set<ClientData>();
        public DbSet<ClientPasportData> ClientPasportData => Set<ClientPasportData>();
        public DbSet<TokenData> TokensData => Set<TokenData>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
                // Визначення one-to-one відношення
                modelBuilder.Entity<ClientData>()
                .HasOne(c => c.ClientPasportData)
                .WithOne(p => p.ClientData)
                .HasForeignKey<ClientData>(c => c.PasportID)
                .OnDelete(DeleteBehavior.SetNull);

                base.OnModelCreating(modelBuilder);
        }
    }
}
