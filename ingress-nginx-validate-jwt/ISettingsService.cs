using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace ingress_nginx_validate_jwt
{
    public interface ISettingsService
    {
        Task<OpenIdConnectConfiguration> GetConfiguration(CancellationToken cancellationToken = default);
    }
}