namespace PitchSwitchBackend.Services.JobExecutor
{
    public interface IJobExecutor
    {
        Task CleanExpiredTokensJob();
    }
}
