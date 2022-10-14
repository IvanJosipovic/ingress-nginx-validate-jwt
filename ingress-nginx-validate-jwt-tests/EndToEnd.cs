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
    public class EndToEnd
    {

        [Fact]
        public async Task Test1()
        {
            var testcontainersBuilder = new TestcontainersBuilder<TestcontainersContainer>()
              .WithImage("ingress-nginx-validate-jwt")
              .WithName("ingress-nginx-validate-jwt")
              .WithEnvironment("OpenIdProviderConfigurationUrl", "https://login.microsoftonline.com/common/v2.0/.well-known/openid-configuration")
              .WithPortBinding(8080)
              .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(8080));

            await using var testcontainers = testcontainersBuilder.Build();
            await testcontainers.StartAsync();


            var result = await new HttpClient().GetAsync("http://localhost:8080/health");

            result.IsSuccessStatusCode.Should().BeTrue();

            var result2 = await new HttpClient().GetAsync("http://localhost:8080/auth");

            result2.StatusCode.Should().Be(HttpStatusCode.Unauthorized);

            var builder = ConfidentialClientApplicationBuilder.Create("40f04710-a912-49f4-b865-47ddf5e8046e")
                .WithClientSecret(Environment.GetEnvironmentVariable("TESTCLIENTSECRET"))
                .WithTenantId("b82075ce-f897-4df8-8624-47b71a1fd251")
                .Build();

            var token = builder.AcquireTokenForClient(new List<string>() { "api://40f04710-a912-49f4-b865-47ddf5e8046e/.default" });
            var accessToken = await token.ExecuteAsync();
            var bearer = accessToken.CreateAuthorizationHeader();

            var result3 = new HttpClient();
            result3.DefaultRequestHeaders.Add("Authorization", bearer);
            var tokenresp = await result3.GetAsync("http://localhost:8080/auth?tid=b82075ce-f897-4df8-8624-47b71a1fd251&aud=api://40f04710-a912-49f4-b865-47ddf5e8046e");
            tokenresp.IsSuccessStatusCode.Should().BeTrue();

            var tokenresp2 = await result3.GetAsync("http://localhost:8080/auth?tid=b82075ce-f897-4df8-8624-47b71a1fd252&aud=api://40f04710-a912-49f4-b865-47ddf5e8046e");
            tokenresp2.IsSuccessStatusCode.Should().BeFalse();

            await testcontainers.StopAsync();
        }
    }
}
