using ingress_nginx_validate_jwt.Services;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace ingress_nginx_validate_jwt_tests;

public class FakeSettingsService : ISettingsService
{
    readonly OpenIdConnectConfiguration config;

    public FakeSettingsService()
    {
        config = new OpenIdConnectConfiguration()
        {
            Issuer = FakeJwtIssuer.Issuer
        };

        config.SigningKeys.Add(FakeJwtIssuer.SecurityKey);
    }

    public Task<OpenIdConnectConfiguration> GetConfiguration(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(config);
    }
}
