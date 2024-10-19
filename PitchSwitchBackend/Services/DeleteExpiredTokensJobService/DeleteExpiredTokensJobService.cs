
using PitchSwitchBackend.Data;

namespace PitchSwitchBackend.Services.DeleteExpiredTokensJob
{
    public class DeleteExpiredTokensJobService : IDeleteExpiredTokensJobService
    {
        private readonly ApplicationDBContext _context;
        public DeleteExpiredTokensJobService(ApplicationDBContext context)
        {
            _context = context;
        }
        public async Task DeleteExpiredRefreshTokensAsync()
        {
            var expiredTokens = _context.RefreshTokens.Where( rt => rt.ExpiryDate < DateTime.UtcNow ).ToList();

            _context.RefreshTokens.RemoveRange(expiredTokens);

            await _context.SaveChangesAsync();
        }
    }
}
