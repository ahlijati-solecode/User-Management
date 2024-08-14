using AutoMapper;
using User_Management.Configurations;
using Xunit;
namespace Shared.Service.Tests.Controllers
{
    public partial class BaseControllerFactoryTest : IClassFixture<WebApiTesterFactory>
    {
        protected IMapper GenerateMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<AutoMapperConfiguration>();
            });
            return config.CreateMapper();
        }
    }
}
