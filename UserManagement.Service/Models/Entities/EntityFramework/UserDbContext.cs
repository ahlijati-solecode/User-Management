using Microsoft.EntityFrameworkCore;
using Shared.Models.Entities;
using Shared.Models.Entities.Configurations;
using Shared.Models.Entities.EntityFramework.Configurations;
using User_Management.Models.Entities.EntityFramework.Configurations;

namespace User_Management.Models.Entities.EntityFramework
{
    public partial class UserDbContext : DbContext
    {
        private readonly ILoggerFactory _loggerFactory;
        public static bool IsFirstLoad { get; set; }
        public UserDbContext(DbContextOptions<UserDbContext> options, ILoggerFactory loggerFactory)
            : base(options)
        {
            if (!IsFirstLoad)
            {
                IsFirstLoad = true;
                Database.Migrate();
            }
            _loggerFactory = loggerFactory;

        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseLoggerFactory(_loggerFactory)  //tie-up DbContext with LoggerFactory object
            .EnableSensitiveDataLogging();
        }
        public virtual DbSet<Shared.Models.Entities.LgActivity> LgActivities { get; set; } = null!;
        public virtual DbSet<MdRole> Roles { get; set; } = null!;
        public virtual DbSet<LgRole> RoleHistories { get; set; } = null!;
        public virtual DbSet<MdEndpoint> Endpoints { get; set; } = null!;
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new LgActivityConfiguration());
            modelBuilder.ApplyConfiguration(new LgRoleConfiguratino());
            modelBuilder.ApplyConfiguration(new LgRoleHistoryConfiguration());
            modelBuilder.ApplyConfiguration(new MdEndpointConfiguration());

            modelBuilder.ApplyConfiguration(new MdRoleAccessConfiguration());
            modelBuilder.ApplyConfiguration(new MdRoleAccessRefConfiguration());
            modelBuilder.ApplyConfiguration(new LgRoleAccessRefConfiguration());
            modelBuilder.ApplyConfiguration(new LgRoleAccessConfiguration());


            modelBuilder.ApplyConfiguration(new TmpRoleUserConfiguration());
            modelBuilder.ApplyConfiguration(new TmpRoleUserRefConfiguration());
            modelBuilder.ApplyConfiguration(new MdRoleUserRefConfiguration());
            modelBuilder.ApplyConfiguration(new MdRoleUserConfiguration());
            modelBuilder.ApplyConfiguration(new LgRoleUserConfiguration());
            modelBuilder.ApplyConfiguration(new LgRoleUserRefConfiguration());


            modelBuilder.ApplyConfiguration(new MdRefMenuConfiguration());

            modelBuilder.ApplyConfiguration(new TsTaskListConfigurations());


        }
    }
}