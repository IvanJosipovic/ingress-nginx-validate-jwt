using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace ingress_nginx_validate_jwt;

public class HostedService : IHostedService
{
    private ILogger<HostedService> _logger;

    private ISettingsService _settingsService;

    public HostedService(ILogger<HostedService> logger, ISettingsService settingsService)
    {
        _logger = logger;
        _settingsService = settingsService;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Version: {version}", FileVersionInfo.GetVersionInfo(GetType().Assembly.Location).ProductVersion);
        _logger.LogInformation("Preloading Configuration");
        await _settingsService.GetConfiguration(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
