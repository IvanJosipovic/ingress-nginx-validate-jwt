using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using FluentAssertions;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ingress_nginx_validate_jwt_tests
{
    public class EndToEndBase : IDisposable
    {
        TestcontainersContainer testcontainersContainer;

        public EndToEndBase()
        {
            var testcontainersBuilder = new TestcontainersBuilder<TestcontainersContainer>()
              .WithImage("ingress-nginx-validate-jwt")
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
