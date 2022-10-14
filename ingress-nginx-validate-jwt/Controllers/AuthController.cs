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

    private JwtSecurityTokenHandler _jwtSecurityTokenHandler;

    private static readonly Gauge Authorized = Metrics.CreateGauge("ingress_nginx_validate_jwt_authorized", "Number of Authorized operations ongoing.");

    private static readonly Gauge Unauthorized = Metrics.CreateGauge("ingress_nginx_validate_jwt_unauthorized", "Number of Unauthorized operations ongoing.");

    private static readonly Histogram ValidationDuration = Metrics.CreateHistogram("ingress_nginx_validate_jwt_duration_seconds", "Histogram of JWT validation durations.");

    public AuthController(ILogger<AuthController> logger, ISettingsService settingsService, JwtSecurityTokenHandler jwtSecurityTokenHandler)
    {
        _logger = logger;
        _settingsService = settingsService;
        _jwtSecurityTokenHandler = jwtSecurityTokenHandler;
    }

    [HttpGet]
    public async Task<ActionResult> Get(CancellationToken cancellationToken)
    {
        using (ValidationDuration.NewTimer())
        {
            try
            {
                var token = Request.Headers.Authorization.FirstOrDefault();

                if (string.IsNullOrEmpty(token))
                {
                    Unauthorized.Inc();
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

                _jwtSecurityTokenHandler.ValidateToken(token, parameters, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;

                foreach (var item in Request.Query)
                {
                    var claim = jwtToken.Claims.First(x => x.Type == item.Key).Value;

                    if (!item.Value.Contains(claim))
                    {
                        Unauthorized.Inc();
                        return Unauthorized();
                    }
                }

                Authorized.Inc();
                return Ok();
            }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AuthController Exception");
            }

                Unauthorized.Inc();
                return Unauthorized();
            }
        }
    }
}