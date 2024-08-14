using AutoMapper;
using Shared.Helpers;
using Shared.Infrastructures.Repositories;
using Shared.Infrastructures.Repositories.Configuration;
using Shared.Infrastructures.Repositories.EndPoint;
using Shared.Infrastructures.Services;
using Shared.Service.Infrastructures.Services.Task;
using User_Management.Configurations;
using User_Management.HostedServices;
using User_Management.Infrastructures.Repositories;
using User_Management.Infrastructures.Repositories.RefMenu;
using User_Management.Infrastructures.Repositories.Role;
using User_Management.Infrastructures.Repositories.RoleUser;
using User_Management.Infrastructures.Repositories.Tasks.Aggregate;
using User_Management.Infrastructures.Repositories.UserAccess;
using User_Management.Infrastructures.Services;
using User_Management.Infrastructures.Services.AcitivityLogs;
using User_Management.Infrastructures.Services.Role;
using User_Management.Infrastructures.Services.Tasks;
using User_Management.Infrastructures.Utils.Report;
using User_Management.Models.Entities.EntityFramework;

namespace User_Management
{
    public class Startup : BaseStartup
    {
        const string ASPNETCORE_ENVIRONMENT = "ASPNETCORE_ENVIRONMENT";
        const string MUFG_ENVIRONMENTS = "test|prod";
        public override string ServiceName => "User";
        public Startup(IConfiguration configuration) : base(configuration)
        {
            BaseStartup.CurrentServiceName = ServiceName;
        }
        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<UserDbContext>(Configuration);
            services.Configure<EndpointSettings>(Configuration.GetSection("Endpoints"));
            base.ConfigureServices(services);
        }

        public override void AddRepositories(IServiceCollection services)
        {
            services.AddScoped<IActivityRepository, EFActivityRepository>();
            services.AddScoped<IRoleRepository, EFRoleRepostitory>();
            services.AddScoped<IConfigurationRepository, RestApiConfigurationRepository>();
            services.AddScoped<IRoleHisotryRepository, EFRoleHistoryRepostitory>();
            services.AddScoped<IUserRepository, JwtUserRepository>();
            services.AddScoped<IEndpointRepository, EfEndpointRepository>();
            if (Environment.GetEnvironmentVariable(ASPNETCORE_ENVIRONMENT) != null && MUFG_ENVIRONMENTS.Contains(Environment.GetEnvironmentVariable(ASPNETCORE_ENVIRONMENT)))
            {
                services.AddScoped<ISearchUserRepository, LdapConsoleMufgUserRepository>();
                services.AddScoped<IAuthenticationRepository, LdapMufgUserRepository>();
            }
            else
            {
                services.AddScoped<ISearchUserRepository, LdapConsoleUserRepository>();
                services.AddScoped<IAuthenticationRepository, LdapUserRepository>();
            }
            services.AddScoped<IRefMenuRepository, EFRefMenuRepostitory>();

            services.AddScoped<IRoleAccessRepository, EFUserAccessRepository>();
            services.AddScoped<IRoleAccessLogRepository, EFUserAccessLogRepository>();
            services.AddScoped<IRoleAccessRefLogRepository, EFUserAccessRefLogRepository>();
            services.AddScoped<IRoleAccessRefRepository, EFUserAccessRefRepository>();

            services.AddScoped<IRoleUserRefRepository, EFRoleUserRefRepository>();
            services.AddScoped<IRoleUserRepository, EFRoleUserRepository>();
            services.AddScoped<ITemporaryRoleUserRepository, EFTemporaryRoleUserRepository>();
            services.AddScoped<ITemporaryRoleUserRefRepository, EFTemporaryRoleUserRefRepository>();
            services.AddScoped<ILogRoleUserRefRepository, EFLogRoleUserRefRepository>();
            services.AddScoped<ILogRoleUserRepository, EFLogRoleUserRepository>();


            services.AddScoped<IActivityRepository, EFActivityRepository>();
            services.AddScoped<IActivityReportProvider, NPoiAcitivyReportProvider>();

            services.AddScoped<ITaskCountRepository, RestApiConfigurationTaskCountingRepository>();

        }

        public override void AddServices(IServiceCollection services)
        {
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IRoleAccessService, RoleAccessService>();
            services.AddScoped<IRoleUserService, RoleUserService>();
            services.AddScoped<IActivityService, ActivityService>();
            services.AddScoped<ITaskCountService, TaskCountService>();
            services.AddScoped<ITaskService, TaskService>();
            services.AddHostedService<CleanUpTemporaryTableHostedService>();
            services.AddHostedService<DatabaseInitilizerHostedService>();
            
        }
        public override void ConfigureAutoMapper(IMapperConfigurationExpression configuration)
        {
            configuration.AddProfile<AutoMapperConfiguration>();
        }
    }
}
