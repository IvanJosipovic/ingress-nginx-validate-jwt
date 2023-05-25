using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Web;
using ingress_nginx_validate_jwt.Constants;
using ingress_nginx_validate_jwt.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using Prometheus;

namespace ingress_nginx_validate_jwt.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    private readonly ILogger<AuthController> _logger;

    private readonly ISettingsService _settingsService;

    private readonly JwtSecurityTokenHandler _jwtSecurityTokenHandler;

    private static readonly Gauge AuthorizedGauge = Metrics.CreateGauge("ingress_nginx_validate_jwt_authorized", "Number of Authorized operations ongoing.");

    private static readonly Gauge UnauthorizedGauge = Metrics.CreateGauge("ingress_nginx_validate_jwt_unauthorized", "Number of Unauthorized operations ongoing.");

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
                var token = GetToken();

                if (!string.IsNullOrWhiteSpace(token))
                {
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
                        if (item.Key.Equals("inject-claim", StringComparison.InvariantCultureIgnoreCase))
                        {
                            foreach (var value in item.Value)
                            {
                                if (value == null)
                                {
                                    continue;
                                }

                                string claimName;
                                string headerName;

                                if (value.Contains(','))
                                {
                                    claimName = value.Split(',')[0];
                                    headerName = value.Split(',')[1];
                                }
                                else
                                {
                                    claimName = value;
                                    headerName = value;
                                }

                                var claims = jwtToken.Claims.Where(x => x.Type == claimName).ToArray();

                                if (claims == null || claims.Length == 0)
                                {
                                    continue;
                                }

                                if (claims.Length == 1)
                                {
                                    Response.Headers.Add(headerName, claims[0].Value);
                                }
                                else
                                {
                                    Response.Headers.Add(headerName, JsonSerializer.Serialize(claims.Select(x => x.Value)));
                                }
                            }
                        }
                        else if (!jwtToken.Claims.Any(x => x.Type == item.Key && item.Value.Any(y => y?.Equals(x.Value) == true)))
                        {
                            UnauthorizedGauge.Inc();
                            return Unauthorized();
                        }
                    }

                    AuthorizedGauge.Inc();
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AuthController Exception");
            }

            UnauthorizedGauge.Inc();
            return Unauthorized();
        }
    }

    private string? GetToken()
    {
        if (AuthenticationHeaderValue.TryParse(Request.Headers.Authorization.FirstOrDefault(), out var header) && header.Scheme == "Bearer")
        {
            return header?.Parameter;
        }
        else if (Request.Headers.TryGetValue(CustomHeaders.OriginalUrl, out StringValues values) && values[0] != null)
        {
            var queryParams = HttpUtility.ParseQueryString(new Uri(values[0]!).Query);
            return queryParams[QueryParameters.AccessToken];
        }

        return null;
    }
}