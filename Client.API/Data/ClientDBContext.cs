using BookingLibrary.Data.DAO;
using BookingLibrary.Helpers.Hash.HashTypes;
using BookingLibrary.JsonResponce;
using Microsoft.EntityFrameworkCore;

namespace Client.API.Data
{
    public class ClientDBContext : DbContext
    {
        public ClientDBContext(DbContextOptions<ClientDBContext> options) : base(options) { }

        public DbSet<ClientDAO> ClientData => Set<ClientDAO>();
        public DbSet<ClientPasportDAO> ClientPasportData => Set<ClientPasportDAO>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public static async Task<ClientDAO> AuthAsync(string login, string password, ClientDBContext context)
        {
            try
            {
                
                ClientDAO? clientData = await context.ClientData.FirstOrDefaultAsync( c => c.Nickname.Equals(login) );
                if (clientData == null) throw ResponseFormat.AUTH_INVALID.Exception;
                string enteredDerivedKey = new Md5Hash().HashString(clientData.Salt + password); 
                if ( !enteredDerivedKey.Equals( clientData.DerivedKey ) ) throw ResponseFormat.AUTH_INVALID.Exception;
                return clientData;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

    }
}
