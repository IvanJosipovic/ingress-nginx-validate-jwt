using System.Diagnostics;

namespace ingress_nginx_validate_jwt.Services;

public class HostedService : IHostedService
{
    private readonly ILogger<HostedService> _logger;

    private readonly ISettingsService _settingsService;

    public HostedService(ILogger<HostedService> logger, ISettingsService settingsService)
    {
        _logger = logger;
        _settingsService = settingsService;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Version: {version}", FileVersionInfo.GetVersionInfo(Process.GetCurrentProcess().MainModule!.FileName!).ProductVersion);
        await _settingsService.GetConfiguration(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}
