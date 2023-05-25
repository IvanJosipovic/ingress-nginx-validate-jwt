using ingress_nginx_validate_jwt.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Protocols;

namespace ingress_nginx_validate_jwt_tests;

public class SettingServiceTest
{
    [Fact]
    public async Task Test()
    {
        IServiceCollection services = new ServiceCollection();

        var configRetriever = new Mock<IConfigurationRetriever<OpenIdConnectConfiguration>>();
        configRetriever.Setup(c => c.GetConfigurationAsync(It.IsAny<string>(), It.IsAny<IDocumentRetriever>(), It.IsAny<CancellationToken>())).Verifiable();

        var config = new ConfigurationBuilder().AddInMemoryCollection(new Dictionary<string, string>() { { "OpenIdProviderConfigurationUrl", "https://demo" } }).Build();
        services.AddSingleton<IConfiguration>(config);
        services.AddLogging();
        services.AddSingleton<ILoggerFactory, NullLoggerFactory>();
        services.AddSingleton(configRetriever.Object);
        services.AddSingleton<ISettingsService, SettingsService>();

        var serviceProvider = services.BuildServiceProvider();

        var settingsService = serviceProvider.GetRequiredService<ISettingsService>();

        await settingsService.GetConfiguration();

        configRetriever.Verify(x => x.GetConfigurationAsync("https://demo", It.IsAny<HttpDocumentRetriever>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}