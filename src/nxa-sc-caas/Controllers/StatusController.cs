using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using NXA.SC.Caas.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Threading.Tasks;
using System.Net;
using System;
using Newtonsoft.Json;

namespace NXA.SC.Caas.Controllers {
    [ApiController]
    [AllowAnonymous]
    [Route("[controller]")]
    public class StatusController : ControllerBase 
    {
        private readonly ILogger<StatusController> _logger;
        private readonly HealthCheckService _healthCheckService; 
        public static readonly DateTime StartupTime = DateTime.Now;

        public StatusController(
            ILogger<StatusController> logger, HealthCheckService healthCheckService
        )
        {
            _healthCheckService = healthCheckService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetServiceStatus()
        {
            var request = HttpContext.Request;
            _logger.LogTrace($"{request.Method} {request.Path}");
            var report = await _healthCheckService.CheckHealthAsync();
            _logger.LogInformation(report.Status.ToString());

            var upTime = DateTime.Now - StartupTime;
            var uptimeJson= JsonConvert.SerializeObject(upTime);

            var result = new SystemStatus
            {
                Uptime = uptimeJson,
                HealthInfo = report
            };

            return report.Status == HealthStatus.Healthy ? Ok(result) :
            StatusCode((int)HttpStatusCode.ServiceUnavailable, result);
        }

    }
}
