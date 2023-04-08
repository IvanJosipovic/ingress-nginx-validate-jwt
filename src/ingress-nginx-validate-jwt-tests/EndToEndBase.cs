using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Images;

namespace ingress_nginx_validate_jwt_tests
{
    public class EndToEndBase : IDisposable
    {
        readonly IContainer container;

        readonly IFutureDockerImage image;

        public EndToEndBase()
        {
            image = new ImageFromDockerfileBuilder()
                .WithDockerfileDirectory("../../../../ingress-nginx-validate-jwt")
                .WithName($"ingress-nginx-validate-jwt-tests:{Guid.NewGuid()}")
                .Build();

            image.CreateAsync().Wait();

            container = new ContainerBuilder()
              //.WithImage("ingress-nginx-validate-jwt")
              .WithImage(image.FullName)
              .WithEnvironment("OpenIdProviderConfigurationUrl", "https://login.microsoftonline.com/common/v2.0/.well-known/openid-configuration")
              .WithPortBinding(8080)
              .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(8080))
              .Build();

            container.StartAsync().Wait();
        }

        public void Dispose()
        {
            container.StopAsync().Wait();
            image.DeleteAsync().Wait();
        }
    }
}
