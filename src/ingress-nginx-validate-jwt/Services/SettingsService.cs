using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace ingress_nginx_validate_jwt.Services;

public class SettingsService : ISettingsService
{
    private readonly ILogger<SettingsService> _logger;

    private readonly IConfiguration _configuration;

    private readonly IConfigurationRetriever<OpenIdConnectConfiguration> configurationRetriever;

    private OpenIdConnectConfiguration? openIdConnectConfiguration;

    public SettingsService(ILogger<SettingsService> logger, IConfiguration configuration, IConfigurationRetriever<OpenIdConnectConfiguration> configurationRetriever)
    {
        _logger = logger;
        _configuration = configuration;
        this.configurationRetriever = configurationRetriever;
    }

    public async Task<OpenIdConnectConfiguration> GetConfiguration(CancellationToken cancellationToken = new CancellationToken())
    {
        if (openIdConnectConfiguration == null)
        {
            string? configEndpoint = _configuration["OpenIdProviderConfigurationUrl"];

            if (string.IsNullOrEmpty(configEndpoint))
            {
                const string exp = "Environment Variable OpenIdProviderConfigurationUrl is null!";

                _logger.LogCritical(exp);

                throw new Exception(exp);
            }

            _logger.LogInformation("Loading OpenIdConfiguration from: {config}", configEndpoint);

            try
            {
                HttpClientHandler handler = new HttpClientHandler();
                bool disableSslVerification = bool.TryParse(_configuration["disableSslVerification"], out bool result) && result;

                if (disableSslVerification)
                {
                    _logger.LogWarning("SSL certificate validation is disabled due to disableSslVerification being set to true!");
                    
                    handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
                }

                var httpClient = new HttpClient(handler);
                var documentRetriever = new HttpDocumentRetriever(httpClient);

                openIdConnectConfiguration = await configurationRetriever.GetConfigurationAsync(configEndpoint, documentRetriever, cancellationToken);
            }
            catch (Exception ex)
            {
                string exp = $"Unable to load OpenIdConfiguration from: {configEndpoint}";

                _logger.LogCritical(ex, exp);

                throw new Exception(exp);
            }
        }

        return openIdConnectConfiguration;
    }
}
