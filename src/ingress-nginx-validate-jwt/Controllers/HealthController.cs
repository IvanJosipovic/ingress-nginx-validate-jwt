using Microsoft.AspNetCore.Mvc;

namespace ingress_nginx_validate_jwt.Controllers;

[ApiController]
[Route("[controller]")]
public class HealthController : ControllerBase
{
    private readonly ILogger<HealthController> _logger;

    private ISettingsService _settingsService;

    public HealthController(ILogger<HealthController> logger, ISettingsService settingsService)
    {
        _logger = logger;
        _settingsService = settingsService;
    }

    [HttpGet]
    public async Task<ActionResult> Get(CancellationToken cancellationToken)
    {
        await _settingsService.GetConfiguration(cancellationToken);

        return Ok();
    }
}