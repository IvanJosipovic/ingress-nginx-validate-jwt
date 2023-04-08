using System.Net;
using ingress_nginx_validate_jwt.Constants;
using Microsoft.Identity.Client;

namespace ingress_nginx_validate_jwt_tests
{
    public class EntToEndTests : IClassFixture<EndToEndBase>
    {
        [Fact]
        public async Task TestHealth()
        {
            using var result = await new HttpClient().GetAsync("http://localhost:8080/health");

            result.IsSuccessStatusCode.Should().BeTrue();
        }

        [Fact]
        public async Task TestUnauthorized()
        {
            using var result = await new HttpClient().GetAsync("http://localhost:8080/auth");

            result.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        [Fact]
        public async Task TestTokenAsAuthorizationHeader()
        {
            var accessToken = await GetValidAuthenticationResult();
            var bearer = accessToken.CreateAuthorizationHeader();

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", bearer);

            var tokenresp = await client.GetAsync("http://localhost:8080/auth?tid=b82075ce-f897-4df8-8624-47b71a1fd251&aud=api://40f04710-a912-49f4-b865-47ddf5e8046e");
            tokenresp.IsSuccessStatusCode.Should().BeTrue();

            var tokenresp2 = await client.GetAsync("http://localhost:8080/auth?tid=b82075ce-f897-4df8-8624-47b71a1fd252&aud=api://40f04710-a912-49f4-b865-47ddf5e8046e");
            tokenresp2.IsSuccessStatusCode.Should().BeFalse();
        }

        [Fact]
        public async Task TestTokenAsQueryParameter()
        {
            var authResult = await GetValidAuthenticationResult();
            using var client = new HttpClient();
            var originalUrlBuilder = new UriBuilder("https://www.example.com") { Query = $"{QueryParameters.AccessToken}={authResult.AccessToken}" };
            client.DefaultRequestHeaders.Add(CustomHeaders.OriginalUrl, originalUrlBuilder.Uri.ToString());

            var tokenresp = await client.GetAsync("http://localhost:8080/auth?tid=b82075ce-f897-4df8-8624-47b71a1fd251&aud=api://40f04710-a912-49f4-b865-47ddf5e8046e");
            tokenresp.IsSuccessStatusCode.Should().BeTrue();

            var tokenresp2 = await client.GetAsync("http://localhost:8080/auth?tid=b82075ce-f897-4df8-8624-47b71a1fd252&aud=api://40f04710-a912-49f4-b865-47ddf5e8046e");
            tokenresp2.IsSuccessStatusCode.Should().BeFalse();
        }

        [Fact]
        public async Task TestInvalidTokenAsAuthorizationHeader()
        {
            var authResult = await GetValidAuthenticationResult();

            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", authResult.AccessToken + "BAD");

            var tokenresp = await client.GetAsync("http://localhost:8080/auth");
            tokenresp.IsSuccessStatusCode.Should().BeFalse();
        }

        [Fact]
        public async Task TestInvalidTokenAsQueryParameter()
        {
            var authResult = await GetValidAuthenticationResult();

            using var client = new HttpClient();
            var originalUrlBuilder = new UriBuilder("https://www.example.com") { Query = $"{QueryParameters.AccessToken}={authResult.AccessToken + "BAD"}" };
            client.DefaultRequestHeaders.Add(CustomHeaders.OriginalUrl, originalUrlBuilder.Uri.ToString());

            var tokenresp = await client.GetAsync("http://localhost:8080/auth");
            tokenresp.IsSuccessStatusCode.Should().BeFalse();
        }

        private static Task<AuthenticationResult> GetValidAuthenticationResult()
        {
            var builder = ConfidentialClientApplicationBuilder.Create("40f04710-a912-49f4-b865-47ddf5e8046e")
                            .WithClientSecret(Environment.GetEnvironmentVariable("TESTCLIENTSECRET"))
                            .WithTenantId("b82075ce-f897-4df8-8624-47b71a1fd251")
                            .Build();

            var token = builder.AcquireTokenForClient(new List<string>() { "api://40f04710-a912-49f4-b865-47ddf5e8046e/.default" });
            return token.ExecuteAsync();
        }
    }
}
