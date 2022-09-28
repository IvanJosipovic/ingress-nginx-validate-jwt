using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace ingress_nginx_validate_jwt;

public class SettingsService
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
                var exp = new Exception("Unable to load OpenIdConfiguration");

                _logger.LogError(exp, "Unable to load OpenIdConfiguration");

                throw exp;
            }

            _logger.LogInformation("Loading OpenIdConfiguration from : {config}", configEndpoint);

            openIdConnectConfiguration = await OpenIdConnectConfigurationRetriever.GetAsync(configEndpoint, cancellationToken);
        }

        return openIdConnectConfiguration;
    }
}
