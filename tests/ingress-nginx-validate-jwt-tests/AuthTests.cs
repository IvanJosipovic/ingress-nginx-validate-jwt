using ingress_nginx_validate_jwt.Constants;
using ingress_nginx_validate_jwt.Controllers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ingress_nginx_validate_jwt_tests;

public class AuthTests
{
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
                new List<KeyValuePair<string, string>>()
                {
                    { new KeyValuePair < string, string >("tid", "11111111-1111-1111-1111-111111111111") }
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
                new List<KeyValuePair<string, string>>()
                {
                    { new KeyValuePair < string, string >("tid", "11111111-1111-1111-1111-111111111111") },
                    { new KeyValuePair < string, string >("aud", "22222222-2222-2222-2222-222222222222") },
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
                new List<KeyValuePair<string, string>>()
                {
                    { new KeyValuePair < string, string >("tenant", "11111111-1111-1111-1111-111111111111") }
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
                new List<KeyValuePair<string, string>>()
                {
                    { new KeyValuePair<string, string>("tenant", "11111111-1111-1111-1111-111111111111" ) },
                    { new KeyValuePair<string, string>("audiance", "22222222-2222-2222-2222-222222222222") },
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
                new List<KeyValuePair<string, string>>()
                {
                    { new KeyValuePair <string, string>("groups", "foo") },
                    { new KeyValuePair <string, string>("groups", "bar") },
                    { new KeyValuePair <string, string>("groups", "baz") },
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

            new object[]
            {
                "?groups=baz",
                new List<Claim>
                {
                    new Claim("groups", "foo"),
                    new Claim("groups", "bar"),
                },
                typeof(UnauthorizedResult)
            },
        };
    }

    public static IEnumerable<object[]> GetTokenAsQueryParameterTests()
    {
        return new List<object[]>
        {
            new object[] // Token Only in Query String
            {
                "",
                new List<Claim>(),
                typeof(OkResult),
                true,
                null!,
                new Dictionary<string, string>()
                {
                    {CustomHeaders.OriginalUrl, $"https://www.example.com?{QueryParameters.AccessToken}={FakeJwtIssuer.GenerateJwtToken(Enumerable.Empty<Claim>())}" }
                }
            },
            new object[] // Token in Query String and Header, Header is used
            {
                "",
                new List<Claim>(),
                typeof(OkResult),
                false,
                null!,
                new Dictionary<string, string>()
                {
                    {CustomHeaders.OriginalUrl, $"https://www.example.com?{QueryParameters.AccessToken}={FakeJwtIssuer.GenerateJwtToken(Enumerable.Empty<Claim>())}" }
                }
            },
            new object[] // Token in Header and Query String with no Token
            {
                "",
                new List<Claim>(),
                typeof(OkResult),
                false,
                null!,
                new Dictionary<string, string>()
                {
                    {CustomHeaders.OriginalUrl, "https://www.example.com" }
                }
            },
            new object[] // Token in Query String Capitalized Header
            {
                "",
                new List<Claim>(),
                typeof(OkResult),
                true,
                null!,
                new Dictionary<string, string>()
                {
                    {CustomHeaders.OriginalUrl.ToUpper(), $"https://www.example.com?{QueryParameters.AccessToken}={FakeJwtIssuer.GenerateJwtToken(Enumerable.Empty<Claim>())}" }
                }
            },
            new object[] // Token in Query String with Claim
            {
                "?tid=11111111-1111-1111-1111-111111111111",
                new List<Claim>(),
                typeof(OkResult),
                true,
                null!,
                new Dictionary<string, string>()
                {
                    {CustomHeaders.OriginalUrl, $"https://www.example.com?{QueryParameters.AccessToken}={FakeJwtIssuer.GenerateJwtToken(new List<Claim>{new Claim("tid", "11111111-1111-1111-1111-111111111111") })}" }
                }
            },
            new object[] // Token in Header with Bad Claim
            {
                "?tid=11111111-1111-1111-1111-111111111111",
                new List<Claim>(),
                typeof(UnauthorizedResult),
                true,
                null!,
                new Dictionary<string, string>()
                {
                    {CustomHeaders.OriginalUrl, $"https://www.example.com?{QueryParameters.AccessToken}={FakeJwtIssuer.GenerateJwtToken(new List<Claim>{new Claim("tid", "22222222-2222-2222-2222-222222222222")})}" }
                }
            },
        };
    }

    [Theory]
    [MemberData(nameof(GetTests))]
    [MemberData(nameof(GetInjectClaimsTests))]
    [MemberData(nameof(GetArrayTests))]
    [MemberData(nameof(GetTokenAsQueryParameterTests))]
    public async Task Tests(string query, List<Claim> claims, Type type, bool nullAuth = false, List<KeyValuePair<string,string?>> expectedHeaders = null, Dictionary<string, string>? requestHeaders = null)
    {
        IdentityModelEventSource.ShowPII = true;

        var settingsService = new FakeSettingsService();

        var httpContext = new DefaultHttpContext();

        if (!nullAuth)
        {
            httpContext.Request.Headers.Authorization = FakeJwtIssuer.GenerateBearerJwtToken(claims);
        }

        if (requestHeaders != null)
        {
            foreach (var header in requestHeaders)
            {
                httpContext.Request.Headers.Add(header.Key, header.Value);
            }
        }

        httpContext.Request.QueryString = new QueryString(query);

        var controllerContext = new ControllerContext()
        {
            HttpContext = httpContext,
        };

        var controller = new AuthController(new Mock<ILogger<AuthController>>().Object, settingsService, new JwtSecurityTokenHandler())
        {
            ControllerContext = controllerContext,
        };

        var result = await controller.Get(new CancellationToken());

        result.Should().BeOfType(type);

        if (expectedHeaders != null)
        {
            foreach (var expectedHeader in expectedHeaders)
            {
                var found = httpContext.Response.Headers.Where(x => x.Key == expectedHeader.Key).SelectMany(x => x.Value).Any(x => x == expectedHeader.Value);
                found.Should().BeTrue();
            }
        }
    }

    [Fact]
    public async Task TestFailure()
    {
        IdentityModelEventSource.ShowPII = true;

        var settingsService = new FakeSettingsService();

        var httpContext = new DefaultHttpContext();

        httpContext.Request.Headers.Authorization = FakeJwtIssuer.GenerateBearerJwtToken(new List<Claim>
            {
                new Claim("tid", "11111111-1111-1111-1111-111111111111")
            }
        );

        httpContext.Request.QueryString = new QueryString("?tid=11111111-1111-1111-1111-111111111111");

        var controllerContext = new ControllerContext()
        {
            HttpContext = httpContext,
        };

        var mockHandler = new Mock<JwtSecurityTokenHandler>();
        SecurityToken token;
        mockHandler.Setup(c => c.ValidateToken(It.IsAny<string>(), It.IsAny<TokenValidationParameters>(), out token)).Throws<Exception>();

        var controller = new AuthController(new Mock<ILogger<AuthController>>().Object, settingsService, mockHandler.Object)
        {
            ControllerContext = controllerContext,
        };

        var result = await controller.Get(new CancellationToken());

        result.Should().BeOfType<UnauthorizedResult>();
    }
}