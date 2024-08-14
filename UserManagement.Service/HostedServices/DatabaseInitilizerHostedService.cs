using Shared.Infrastructures.Repositories;
using User_Management.Infrastructures.Repositories;

namespace User_Management.HostedServices
{
    public class DatabaseInitilizerHostedService : BackgroundService
    {
        private const string INACTIVE_EMAIL_MONITORING = "In active Email Monitoring";
        private readonly IServiceProvider _serviceProvider;

        public DatabaseInitilizerHostedService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var scope = _serviceProvider.CreateScope();
            var menuRepository = scope.ServiceProvider.GetRequiredService<IRefMenuRepository>();
            var unitOfWorks = scope.ServiceProvider.GetRequiredService<IUnitOfWorks>();
            var menuInactive =await menuRepository.GetByName(INACTIVE_EMAIL_MONITORING);
            if (menuRepository == null)
            {
                await menuRepository.AddAsync(new Models.Entities.MdRefMenu()
                {
                    Id = 14,
                    Name = INACTIVE_EMAIL_MONITORING,
                    Key = nameof(INACTIVE_EMAIL_MONITORING)
                }); ;
                await unitOfWorks.SaveChangesAsync();
            }
        }
    }
}
