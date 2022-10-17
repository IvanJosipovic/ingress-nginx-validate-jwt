using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace ingress_nginx_validate_jwt;

public class SettingsService : ISettingsService
{
    private ILogger<SettingsService> _logger;

    private IConfiguration _configuration;

    private OpenIdConnectConfiguration? openIdConnectConfiguration;

    public SettingsService(ILogger<SettingsService> logger, IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    public async Task<OpenIdConnectConfiguration> GetConfiguration(CancellationToken cancellationToken = new CancellationToken())
    {
        if (openIdConnectConfiguration == null)
        {
            string? configEndpoint = _configuration["OpenIdProviderConfigurationUrl"];

            if (string.IsNullOrEmpty(configEndpoint))
            {
                var exp = "Environment Variable OpenIdProviderConfigurationUrl is null!";

                _logger.LogCritical(exp);

                throw new Exception(exp);
            }

            _logger.LogInformation("Loading OpenIdConfiguration from: {config}", configEndpoint);

            try
            {
                openIdConnectConfiguration = await OpenIdConnectConfigurationRetriever.GetAsync(configEndpoint, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogCritical(ex, "Unable to load OpenIdConfiguration from: {config}", configEndpoint);
                throw;
            }
        }

        return openIdConnectConfiguration;
    }
}
