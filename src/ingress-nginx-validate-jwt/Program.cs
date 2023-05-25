using ingress_nginx_validate_jwt.Services;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Protocols;
using Prometheus;
using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;

namespace ingress_nginx_validate_jwt;

[ExcludeFromCodeCoverage]
public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();

        builder.Services.AddSingleton<ISettingsService, SettingsService>();

        builder.Services.AddSingleton<JwtSecurityTokenHandler>();

        builder.Services.AddSingleton<IConfigurationRetriever<OpenIdConnectConfiguration>, OpenIdConnectConfigurationRetriever>();

        builder.Services.AddHostedService<HostedService>();

        builder.Services.AddHealthChecks();

        var app = builder.Build();

        // Configure the HTTP request pipeline.

        app.UseForwardedHeaders();

        app.MapControllers();

        app.UseMetricServer();

        app.MapHealthChecks("/health");

        app.Run();
    }
}