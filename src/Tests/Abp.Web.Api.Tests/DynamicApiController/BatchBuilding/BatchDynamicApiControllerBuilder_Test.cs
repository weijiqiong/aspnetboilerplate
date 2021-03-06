using System.Reflection;
using Adorable.Application.Services;
using Adorable.Dependency;
using Adorable.WebApi.Controllers.Dynamic;
using Adorable.WebApi.Controllers.Dynamic.Builders;
using Shouldly;
using Xunit;

namespace Adorable.Web.Api.Tests.DynamicApiController.BatchBuilding
{
    public class BatchDynamicApiControllerBuilder_Test
    {
        [Fact]
        public void Test1()
        {
            //TODO: This test fails since it use static IocManager. We should use a local IocManager.
            //IocManager.Instance.Register<IMyFirstAppService, MyFirstAppService>();

            //DynamicApiControllerBuilder
            //    .ForAll<IApplicationService>(Assembly.GetExecutingAssembly(), "myapp/ns1")
            //    .Build();

            //DynamicApiControllerManager.GetAll().Count.ShouldBe(1);
            //DynamicApiControllerManager.FindOrNull("myapp/ns1/myFirst").ShouldNotBe(null);
        }
    }

    public interface IMyFirstAppService : IApplicationService
    {
        
    }

    public abstract class MyFirstAppService : IMyFirstAppService
    {
        
    }
}