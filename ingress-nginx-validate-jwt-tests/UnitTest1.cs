using FluentAssertions;
using ingress_nginx_validate_jwt;
using ingress_nginx_validate_jwt.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Moq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Principal;

namespace ingress_nginx_validate_jwt_tests
{
    public class UnitTest1
    {
        // https://stebet.net/mocking-jwt-tokens-in-asp-net-core-integration-tests/
        public static class MockJwtTokens
        {
            public static string Issuer { get; } = Guid.NewGuid().ToString();
            public static SecurityKey SecurityKey { get; }
            public static SigningCredentials SigningCredentials { get; }

            private static readonly JwtSecurityTokenHandler s_tokenHandler = new JwtSecurityTokenHandler();
            private static readonly RandomNumberGenerator s_rng = RandomNumberGenerator.Create();
            private static readonly byte[] s_key = new byte[32];

            static MockJwtTokens()
            {
                s_rng.GetBytes(s_key);
                SecurityKey = new SymmetricSecurityKey(s_key) { KeyId = Guid.NewGuid().ToString() };
                SigningCredentials = new SigningCredentials(SecurityKey, SecurityAlgorithms.HmacSha256);
            }

            public static string GenerateJwtToken(IEnumerable<Claim> claims)
            {
                return s_tokenHandler.WriteToken(new JwtSecurityToken(Issuer, null, claims, null, DateTime.UtcNow.AddMinutes(20), SigningCredentials));
            }
        }

        public static IEnumerable<object[]> GetTests()
        {
            return new List<object[]>
            {
                new object[]
                {
                    "",
                    new List<Claim>
                    {
                    },
                    typeof(OkResult)
                },
                new object[]
                {
                    "",
                    new List<Claim>
                    {
                    },
                    typeof(UnauthorizedResult),
                    true
                },
                new object[]
                {
                    "?tid=47c63aee-0b28-46b6-b8fa-1cfb273e761b",
                    new List<Claim>
                    {
                        new Claim("tid", "47c63aee-0b28-46b6-b8fa-1cfb273e761b")
                    },
                    typeof(OkResult)
                },
                new object[]
                {
                    "?tid=47c63aee-0b28-46b6-b8fa-1cfb273e761b&aud=f2b06abf-ba7f-4fb9-8b14-d38d0d13d607",
                    new List<Claim>
                    {
                        new Claim("tid", "47c63aee-0b28-46b6-b8fa-1cfb273e761b"),
                        new Claim("aud", "f2b06abf-ba7f-4fb9-8b14-d38d0d13d607")
                    },
                    typeof(OkResult)
                },
                new object[]
                {
                    "?tid=47c63aee-0b28-46b6-b8fa-1cfb273e761b&aud=f2b06abf-ba7f-4fb9-8b14-d38d0d13d607&aud=17d5386c-b52a-4a0d-bf2a-89540495b39c",
                    new List<Claim>
                    {
                        new Claim("tid", "47c63aee-0b28-46b6-b8fa-1cfb273e761b"),
                        new Claim("aud", "f2b06abf-ba7f-4fb9-8b14-d38d0d13d607")
                    },
                    typeof(OkResult)
                },
                new object[]
                {
                    "?tid=47c63aee-0b28-46b6-b8fa-1cfb273e761b&aud=f2b06abf-ba7f-4fb9-8b14-d38d0d13d607&aud=17d5386c-b52a-4a0d-bf2a-89540495b39c",
                    new List<Claim>
                    {
                        new Claim("tid", "47c63aee-0b28-46b6-b8fa-1cfb273e761b"),
                        new Claim("aud", "17d5386c-b52a-4a0d-bf2a-89540495b39c")
                    },
                    typeof(OkResult)
                },
                new object[]
                {
                    "?tid=47c63aee-0b28-46b6-b8fa-1cfb273e761b&aud=f2b06abf-ba7f-4fb9-8b14-d38d0d13d607&aud=17d5386c-b52a-4a0d-bf2a-89540495b39c",
                    new List<Claim>
                    {
                    },
                    typeof(UnauthorizedResult)
                },
                new object[]
                {
                    "?tid=47c63aee-0b28-46b6-b8fa-1cfb273e761b&aud=f2b06abf-ba7f-4fb9-8b14-d38d0d13d607&aud=17d5386c-b52a-4a0d-bf2a-89540495b39c",
                    new List<Claim>
                    {
                        new Claim("tid", "47c63aee-0b28-46b6-b8fa-1cfb273e761b")
                    },
                    typeof(UnauthorizedResult)
                },
            };
        }

        [Theory]
        [MemberData(nameof(GetTests))]
        public async Task Test1(string query, List<Claim> claims, Type type, bool nullAuth = false)
        {
            IdentityModelEventSource.ShowPII = true;

            var settingsService = new Mock<ISettingsService>();
            var config = new OpenIdConnectConfiguration()
            {
                Issuer = MockJwtTokens.Issuer
            };
            config.SigningKeys.Add(MockJwtTokens.SecurityKey);

            settingsService.Setup(x => x.GetConfiguration(new CancellationToken())).Returns(Task.FromResult(config));

            var httpContext = new DefaultHttpContext();
            if (!nullAuth)
            {
                httpContext.Request.Headers.Authorization = MockJwtTokens.GenerateJwtToken(claims);
            }
            httpContext.Request.QueryString = new QueryString(query);

            var controllerContext = new ControllerContext()
            {
                HttpContext = httpContext,
            };

            var controller = new AuthController(new Mock<ILogger<AuthController>>().Object, settingsService.Object, new JwtSecurityTokenHandler())
            {
                ControllerContext = controllerContext,
            };

            var result = await controller.Get(new CancellationToken());

            result.Should().BeOfType(type);
        }
    }
}