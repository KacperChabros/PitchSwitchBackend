
using PitchSwitchBackend.Services.DeleteExpiredTokensJob;

namespace PitchSwitchBackend.Services.JobExecutor
{
    public class JobExecutor : IJobExecutor
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger _logger;

        public JobExecutor(IServiceProvider serviceProvider, ILogger<JobExecutor> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        public async Task CleanExpiredTokensJob()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var jobService = scope.ServiceProvider.GetRequiredService<IDeleteExpiredTokensJobService>();
                try
                {
                    await jobService.DeleteExpiredRefreshTokensAsync();
                }
                catch(Exception ex)
                {
                    _logger.LogError($"An error occured while executing CleanExpiredTokensJob:\n{ex.Message}");
                }
            }
        }
    }
}
