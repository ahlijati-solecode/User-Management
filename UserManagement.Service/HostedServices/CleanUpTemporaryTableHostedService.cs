using User_Management.Infrastructures.Repositories.RoleUser;

namespace User_Management.HostedServices
{
    public class CleanUpTemporaryTableHostedService : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<CleanUpTemporaryTableHostedService> _logger;

        public CleanUpTemporaryTableHostedService(IServiceScopeFactory serviceScopeFactory, ILogger<CleanUpTemporaryTableHostedService> logger)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
        }
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var timer = new PeriodicTimer(TimeSpan.FromHours(1));
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                try
                {
                    _logger.LogInformation($"Starting CleanUp Temp {DateTime.Now}");
                    using var scope = _serviceScopeFactory.CreateScope();
                    var temporaryRoleUserRepository = scope.ServiceProvider.GetService<ITemporaryRoleUserRepository>();
                    if (temporaryRoleUserRepository != null)
                        await temporaryRoleUserRepository.CleanUp();
                    _logger.LogInformation($"Done CleanUp Temp {DateTime.Now}");
                }
                catch (Exception ex)
                {
                    _logger.LogError($"{nameof(CleanUpTemporaryTableHostedService)}", ex);
                }


            }
        }
    }
}
