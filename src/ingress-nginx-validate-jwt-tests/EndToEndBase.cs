using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;

namespace ingress_nginx_validate_jwt_tests
{
    public class EndToEndBase : IDisposable
    {
        TestcontainersContainer testcontainersContainer;

        public EndToEndBase()
        {
            var testcontainersBuilder = new TestcontainersBuilder<TestcontainersContainer>()
              .WithImage("ivanjosipovic/ingress-nginx-validate-jwt")
              .WithName("ingress-nginx-validate-jwt")
              .WithEnvironment("OpenIdProviderConfigurationUrl", "https://login.microsoftonline.com/common/v2.0/.well-known/openid-configuration")
              .WithPortBinding(8080)
              .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(8080));

            testcontainersContainer = testcontainersBuilder.Build();
            testcontainersContainer.StartAsync().Wait();
        }

        public void Dispose()
        {
            testcontainersContainer.StopAsync().Wait();
        }
    }
}
