using User_Management;

namespace Service
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
			.ConfigureLogging(configure =>
                {
                    var serviceDescriptor = configure.Services.FirstOrDefault(descriptor => descriptor.ServiceType == typeof(ILoggerProvider)
                    && descriptor?.ImplementationType?.Name == "ConsoleLoggerProvider");
                    if (serviceDescriptor != null)
                        configure.Services.Remove(serviceDescriptor);

                })
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });
    }
}
