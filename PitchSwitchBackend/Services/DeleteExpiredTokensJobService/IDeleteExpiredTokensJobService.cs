namespace PitchSwitchBackend.Services.DeleteExpiredTokensJob
{
    public interface IDeleteExpiredTokensJobService
    {
        Task DeleteExpiredRefreshTokensAsync();
    }
}
