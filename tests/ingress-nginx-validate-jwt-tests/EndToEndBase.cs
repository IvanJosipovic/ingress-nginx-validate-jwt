using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using System.Net;

namespace ingress_nginx_validate_jwt_tests;

public class EndToEndBase : IDisposable
{
    readonly IContainer container;

    public EndToEndBase()
    {
        container = new ContainerBuilder()
          .WithImage("ingress-nginx-validate-jwt")
          .WithEnvironment("OpenIdProviderConfigurationUrl", "https://login.microsoftonline.com/common/v2.0/.well-known/openid-configuration")
          .WithPortBinding(8080)
          .WithWaitStrategy(Wait.ForUnixContainer().UntilHttpRequestIsSucceeded(request => request.ForPath("/health").ForPort(8080)))
          .Build();

        container.StartAsync().Wait();
    }

    public void Dispose()
    {
        container.StopAsync().Wait();
    }
}
