using ingress_nginx_validate_jwt.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Hosting;

namespace ingress_nginx_validate_jwt_tests;

public class HostedServiceTests
{
    [Fact]
    public async Task Test()
    {
        IServiceCollection services = new ServiceCollection();

        var settingsService = new Mock<ISettingsService>();
        settingsService.Setup(c => c.GetConfiguration(It.IsAny<CancellationToken>())).Verifiable();

        services.AddLogging();
        services.AddSingleton<ILoggerFactory, NullLoggerFactory>();
        services.AddSingleton(settingsService.Object);
        services.AddHostedService<HostedService>();

        var serviceProvider = services.BuildServiceProvider();

        var service = serviceProvider.GetService<IHostedService>() as HostedService;

        await service!.StartAsync(CancellationToken.None);

        await service.StopAsync(CancellationToken.None);

        settingsService.Verify(x => x.GetConfiguration(It.IsAny<CancellationToken>()), Times.Once);
    }
}