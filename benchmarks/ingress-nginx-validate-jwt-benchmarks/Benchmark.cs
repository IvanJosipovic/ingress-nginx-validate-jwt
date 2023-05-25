using BenchmarkDotNet.Attributes;
using ingress_nginx_validate_jwt.Controllers;
using Microsoft.Extensions.Logging;
using System.IdentityModel.Tokens.Jwt;
using ingress_nginx_validate_jwt_tests;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using ingress_nginx_validate_jwt.Services;

namespace ingress_nginx_validate_jwt_benchmarks;

[MemoryDiagnoser]
public class Benchmark
{
    ILogger<AuthController> logger = null!;

    ISettingsService settingsService = null!;

    string token = null!;

    [GlobalSetup]
    public void GlobalSetup()
    {
        var loggerFactory = LoggerFactory.Create(loggingBuilder => loggingBuilder
            .SetMinimumLevel(LogLevel.Information)
            .AddConsole());

        logger = loggerFactory.CreateLogger<AuthController>();

        settingsService = new FakeSettingsService();

        token = FakeJwtIssuer.GenerateBearerJwtToken(new List<Claim>
            {
                new Claim("tid", "11111111-1111-1111-1111-111111111111")
            }
        );
    }

    [Benchmark]
    public async Task Auth()
    {
        var httpContext = new DefaultHttpContext();

        httpContext.Request.Headers.Authorization = token;

        var controllerContext = new ControllerContext()
        {
            HttpContext = httpContext,
        };

        httpContext.Request.QueryString = new QueryString("?tid=11111111-1111-1111-1111-111111111111");

        var controller = new AuthController(logger, settingsService, new JwtSecurityTokenHandler())
        {
            ControllerContext = controllerContext,
        };

        var result = await controller.Get(new CancellationToken());

        if (result is not OkResult)
        {
            throw new Exception("Result should be OK");
        }
    }
}