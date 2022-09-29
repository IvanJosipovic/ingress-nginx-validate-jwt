using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Prometheus;
using System.IdentityModel.Tokens.Jwt;

namespace ingress_nginx_validate_jwt.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly ILogger<AuthController> _logger;

    private ISettingsService _settingsService;

    private JwtSecurityTokenHandler _jwtSecurityTokenHandler1;

    private static readonly Gauge Authorized = Metrics.CreateGauge("ingress_nginx_validata_jwt_authorized", "Number of Authorized operations ongoing.");

    private static readonly Gauge Unauthorized = Metrics.CreateGauge("ingress_nginx_validata_jwt_anauthorized", "Number of Unauthorized operations ongoing.");

    public AuthController(ILogger<AuthController> logger, ISettingsService settingsService, JwtSecurityTokenHandler jwtSecurityTokenHandler)
    {
        _logger = logger;
        _settingsService = settingsService;
        _jwtSecurityTokenHandler1 = jwtSecurityTokenHandler;
    }

    [HttpGet]
    public async Task<ActionResult> Get(CancellationToken cancellationToken)
    {
        try
        {
            var token = Request.Headers.Authorization.FirstOrDefault();

            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized();
            }

            // Remove "Bearer "
            if (token.StartsWith("Bearer "))
            {
                token = token.Substring(7);
            }

            var settings = await _settingsService.GetConfiguration(cancellationToken);

            var parameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKeys = settings.SigningKeys,
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.FromSeconds(0)
            };

            _jwtSecurityTokenHandler1.ValidateToken(token, parameters, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;

            foreach (var item in Request.Query)
            {
                var claim = jwtToken.Claims.First(x => x.Type == item.Key).Value;

                if (!item.Value.Contains(claim))
                {
                    return Unauthorized();
                }
            }

            return Ok();
        }
        catch
        {
            return Unauthorized();
        }
    }
}