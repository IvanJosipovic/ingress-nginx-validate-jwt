using ingress_nginx_validate_jwt;
using ingress_nginx_validate_jwt.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace ingress_nginx_validate_jwt_tests
{
    public class AuthTests
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
                return "Bearer " + s_tokenHandler.WriteToken(new JwtSecurityToken(Issuer, null, claims, null, DateTime.UtcNow.AddMinutes(20), SigningCredentials));
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
                    "?tid=11111111-1111-1111-1111-111111111111",
                    new List<Claim>
                    {
                        new Claim("tid", "11111111-1111-1111-1111-111111111111")
                    },
                    typeof(OkResult)
                },
                new object[]
                {
                    "?tid=11111111-1111-1111-1111-111111111111&aud=22222222-2222-2222-2222-222222222222",
                    new List<Claim>
                    {
                        new Claim("tid", "11111111-1111-1111-1111-111111111111"),
                        new Claim("aud", "22222222-2222-2222-2222-222222222222"),
                    },
                    typeof(OkResult)
                },
                new object[]
                {
                    "?tid=11111111-1111-1111-1111-111111111111&aud=22222222-2222-2222-2222-222222222222&aud=33333333-3333-3333-3333-333333333333",
                    new List<Claim>
                    {
                        new Claim("tid", "11111111-1111-1111-1111-111111111111"),
                        new Claim("aud", "33333333-3333-3333-3333-333333333333")
                    },
                    typeof(OkResult)
                },
                new object[]
                {
                    "?tid=11111111-1111-1111-1111-111111111111&aud=22222222-2222-2222-2222-222222222222&aud=33333333-3333-3333-3333-333333333333",
                    new List<Claim>
                    {
                        new Claim("tid", "11111111-1111-1111-1111-111111111111"),
                        new Claim("aud", "22222222-2222-2222-2222-222222222222"),
                        new Claim("aud", "33333333-3333-3333-3333-333333333333")
                    },
                    typeof(OkResult)
                },


                new object[]
                {
                    "?tid=11111111-1111-1111-1111-111111111111",
                    new List<Claim>
                    {
                    },
                    typeof(UnauthorizedResult)
                },

                new object[]
                {
                    "?tid=11111111-1111-1111-1111-111111111111",
                    new List<Claim>
                    {
                        new Claim("tid", "22222222-2222-2222-2222-222222222222")
                    },
                    typeof(UnauthorizedResult)
                },

                new object[]
                {
                    "?tid=11111111-1111-1111-1111-111111111111&aud=22222222-2222-2222-2222-222222222222&aud=33333333-3333-3333-3333-333333333333",
                    new List<Claim>
                    {
                    },
                    typeof(UnauthorizedResult)
                },

                new object[]
                {
                    "?tid=11111111-1111-1111-1111-111111111111&aud=22222222-2222-2222-2222-222222222222&aud=33333333-3333-3333-3333-333333333333",
                    new List<Claim>
                    {
                        new Claim("tid", "")
                    },
                    typeof(UnauthorizedResult)
                },
            };
        }

        public static IEnumerable<object[]> GetInjectClaimsTests()
        {
            return new List<object[]>
            {
                new object[]
                {
                    "?tid=11111111-1111-1111-1111-111111111111&inject-claim=tid",
                    new List<Claim>
                    {
                        new Claim("tid", "11111111-1111-1111-1111-111111111111")
                    },
                    typeof(OkResult),
                    false,
                    new Dictionary<string, string>()
                    {
                        { "tid", "11111111-1111-1111-1111-111111111111" }
                    }
                },

                new object[]
                {
                    "?tid=11111111-1111-1111-1111-111111111111&inject-claim=tid&inject-claim=aud",
                    new List<Claim>
                    {
                        new Claim("tid", "11111111-1111-1111-1111-111111111111"),
                        new Claim("aud", "22222222-2222-2222-2222-222222222222"),
                    },
                    typeof(OkResult),
                    false,
                    new Dictionary<string, string>()
                    {
                        { "tid", "11111111-1111-1111-1111-111111111111" },
                        { "aud", "22222222-2222-2222-2222-222222222222" },
                    }
                },

                new object[]
                {
                    "?tid=11111111-1111-1111-1111-111111111111&inject-claim=tid,tenant",
                    new List<Claim>
                    {
                        new Claim("tid", "11111111-1111-1111-1111-111111111111"),
                        new Claim("aud", "22222222-2222-2222-2222-222222222222"),
                    },
                    typeof(OkResult),
                    false,
                    new Dictionary<string, string>()
                    {
                        { "tenant", "11111111-1111-1111-1111-111111111111" }
                    }
                },

                new object[]
                {
                    "?tid=11111111-1111-1111-1111-111111111111&inject-claim=tid,tenant&inject-claim=aud,audiance",
                    new List<Claim>
                    {
                        new Claim("tid", "11111111-1111-1111-1111-111111111111"),
                        new Claim("aud", "22222222-2222-2222-2222-222222222222"),
                    },
                    typeof(OkResult),
                    false,
                    new Dictionary<string, string>()
                    {
                        { "tenant", "11111111-1111-1111-1111-111111111111" },
                        { "audiance", "22222222-2222-2222-2222-222222222222" },
                    }
                },

                new object[]
                {
                    "?tid=11111111-1111-1111-1111-111111111111&inject-claim=group",
                    new List<Claim>
                    {
                        new Claim("tid", "11111111-1111-1111-1111-111111111111")
                    },
                    typeof(OkResult)
                },

                new object[]
                {
                    "?tid=11111111-1111-1111-1111-111111111111&inject-claim=group,group",
                    new List<Claim>
                    {
                        new Claim("tid", "11111111-1111-1111-1111-111111111111"),
                        new Claim("aud", "22222222-2222-2222-2222-222222222222"),
                    },
                    typeof(OkResult)
                },
            };
        }

        public static IEnumerable<object[]> GetArrayTests()
        {
            return new List<object[]>
            {
                new object[]
                {
                    "?inject-claim=groups",
                    new List<Claim>
                    {
                        new Claim("groups", "foo"),
                        new Claim("groups", "bar"),
                        new Claim("groups", "baz"),
                    },
                    typeof(OkResult),
                    false,
                    new Dictionary<string, string>()
                    {
                        { "groups", "[\"foo\",\"bar\",\"baz\"]" }
                    }
                },

                new object[]
                {
                    "?groups=foo",
                    new List<Claim>
                    {
                        new Claim("groups", "foo"),
                        new Claim("groups", "bar"),
                        new Claim("groups", "baz"),
                    },
                    typeof(OkResult)
                },

                new object[]
                {
                    "?groups=bar",
                    new List<Claim>
                    {
                        new Claim("groups", "foo"),
                        new Claim("groups", "bar"),
                        new Claim("groups", "baz"),
                    },
                    typeof(OkResult)
                },

                new object[]
                {
                    "?groups=baz",
                    new List<Claim>
                    {
                        new Claim("groups", "foo"),
                        new Claim("groups", "bar"),
                        new Claim("groups", "baz"),
                    },
                    typeof(OkResult)
                },
            };
        }

        [Theory]
        [MemberData(nameof(GetTests))]
        [MemberData(nameof(GetInjectClaimsTests))]
        [MemberData(nameof(GetArrayTests))]
        public async Task Test1(string query, List<Claim> claims, Type type, bool nullAuth = false, Dictionary<string,string> expectedHeaders = null)
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

            if (expectedHeaders != null)
            {
                foreach (var expectedHeader in expectedHeaders)
                {
                    httpContext.Response.Headers[expectedHeader.Key].ToString().Should().Be(expectedHeader.Value);
                }
            }
        }
    }
}