using BookingLibrary.Data.DAO;
using BookingLibrary.Helpers.Hash.HashTypes;
using Microsoft.EntityFrameworkCore;
using Residence.API.Data.DAO;

namespace Residence.Data
{
    public class ResidenceDBContext : DbContext
    {
        public ResidenceDBContext(DbContextOptions<ResidenceDBContext> options) : base(options) { }

        public DbSet<CategoriesDAO> Categories => Set<CategoriesDAO>();
        public DbSet<ResidenceDAO> Residences => Set<ResidenceDAO>();
        public DbSet<ResidenceAdvantagesDAO> ResidenceAdvantages => Set<ResidenceAdvantagesDAO>();
        public DbSet<AdvantagesDAO> Advantages => Set<AdvantagesDAO>();
        public DbSet<PhotosDAO> Photos => Set<PhotosDAO>();
        public DbSet<ApartmentSummaryDAO> ApartmentSummaries => Set<ApartmentSummaryDAO>();
        public DbSet<ApartmentDAO> Apartments => Set<ApartmentDAO>();
        public DbSet<SummaryDAO> Summaries => Set<SummaryDAO>();
        public DbSet<TagApartmentDAO> TagApartments => Set<TagApartmentDAO>();
        public DbSet<TagsDAO> Tags => Set<TagsDAO>();

        public DbSet<ResidenceSearchDAO> ResidencesSearch => Set<ResidenceSearchDAO>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ResidenceAdvantagesDAO>()
             .HasOne(r => r.Residence)
             .WithMany(c => c.ResidenceAdvantages)
             .HasForeignKey(r => r.ResidenceId)
             .OnDelete(DeleteBehavior.Cascade); // Видалити всі зв'язки при видаленні резиденції

            modelBuilder.Entity<ResidenceAdvantagesDAO>()
                .HasOne(a => a.Advantages)
                .WithMany(c => c.ResidenceAdvantages)
                .HasForeignKey(a => a.AdvantagesId)
                .OnDelete(DeleteBehavior.Cascade); // Видалити всі зв'язки при видаленні переваг
            
            modelBuilder.Entity<ApartmentSummaryDAO>()
               .HasOne(a => a.Summary)
               .WithMany(c => c.ApartmentSummaries)
               .HasForeignKey(a => a.SummaryId)
               .OnDelete(DeleteBehavior.Cascade); // Видалити всі зв'язки при видаленні summary_info

            modelBuilder.Entity<TagApartmentDAO>()
               .HasOne(a => a.Tags)
               .WithMany(c => c.TagApartments)
               .HasForeignKey(a => a.TagId)
               .OnDelete(DeleteBehavior.Cascade); // Видалити всі зв'язки при видаленні tag


            base.OnModelCreating(modelBuilder);
        }


    }

}
