using BookingLibrary.Data.DAO;
using BookingLibrary.Helpers.Hash.HashTypes;
using Microsoft.EntityFrameworkCore;

namespace Residence.Data
{
    public class ResidenceDBContext : DbContext
    {
        public ResidenceDBContext(DbContextOptions<ResidenceDBContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }

}
