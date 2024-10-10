using BookingLibrary.Data.DAO;
using BookingLibrary.JsonResponce;
using Microsoft.EntityFrameworkCore;

namespace BookingLibrary.Data
{
    public class TokenDBContext : DbContext
    {
        public TokenDBContext(DbContextOptions<TokenDBContext> options) : base(options) { }
        public DbSet<TokenDAO> TokensData => Set<TokenDAO>();
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public static async Task<TokenDAO?> GetTokenDataAsync(TokenDBContext dbContext, string tokenId)
        {
            TokenDAO? tokenData = await dbContext.TokensData.FirstOrDefaultAsync(t => t.TokenID.Equals(tokenId));
            if (tokenData == null) return null;
            tokenData.TokenUsed = DateTime.UtcNow;
            await dbContext.SaveChangesAsync();
            return tokenData;
        }

        public static async Task<TokenDAO> GenerateTokenAsync(TokenDBContext dbContext)
        {
            TokenDAO? tokenData = new TokenDAO();
            do
            {
                tokenData.TokenID = Guid.NewGuid().ToString();
                tokenData.Salt = Guid.NewGuid().ToString();
                tokenData.TokenCreated = DateTime.UtcNow;
                tokenData.TokenUsed = DateTime.UtcNow;
                tokenData.UserID = null;
            } while (await dbContext.TokensData.FindAsync(tokenData.TokenID) != null);
            await dbContext.AddAsync(tokenData);
            await dbContext.SaveChangesAsync();
            return tokenData;
        }

        public static async Task<TokenDAO?> FindTokenAsync(string tokenId, TokenDBContext context)
        {
            return await context.TokensData.FindAsync(tokenId);
        }
        public static async Task<bool?> DeleteTokenAsync(string tokenId, TokenDBContext context)
        {
            try
            {
                TokenDAO tokenData = await context.TokensData.FindAsync(tokenId) ?? throw ResponseFormat.TOKEN_DATA_NULL.Exception;
                context.TokensData.Remove(tokenData);
                await context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex) 
            {
                return false;
            }
            
        }
        public static async Task<List<TokenDAO>> FindTokensAsync(string userId, TokenDBContext context)
        {
            return await context.TokensData.Where(t => t.UserID.Equals(userId)).ToListAsync();
        }

        public static async Task<TokenDAO> Subscribe(string tokenId, string clientId, string jwt, TokenDBContext context)
        {
            TokenDAO? tokenData = await FindTokenAsync(tokenId, context);
            if (tokenData == null) throw ResponseFormat.TOKEN_ID_INVALID.Exception;
            if (tokenData.UserID != null)
            {
                throw ResponseFormat.LOGIN_AGAIN(jwt).Exception;
            }
            tokenData.UserID = clientId;
            tokenData.TokenUsed = DateTime.UtcNow;
            await context.SaveChangesAsync();
            return tokenData;
        }

        public static async Task<TokenDAO> UnSubscribe(string tokenId, string clientId, TokenDBContext context)
        {
            TokenDAO? tokenData = await FindTokenAsync(tokenId, context);
            if (tokenData == null) throw ResponseFormat.TOKEN_ID_INVALID.Exception;
            tokenData.UserID = null;
            tokenData.TokenUsed = DateTime.UtcNow;
            await context.SaveChangesAsync();
            return tokenData;
        }

    }
}
