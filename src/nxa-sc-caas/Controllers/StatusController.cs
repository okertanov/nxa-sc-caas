using System;
using System.Net;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NXA.SC.Caas.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace NXA.SC.Caas.Controllers
{
    [ApiController]
    [AllowAnonymous]
    [Route("[controller]")]
    public class StatusController : ControllerBase 
    {
        private readonly ILogger<StatusController> logger;
        private readonly HealthCheckService healthCheckService; 
        private readonly DateTime startupTime;

        public StatusController(
            ILogger<StatusController> logger,
            HealthCheckService healthCheckService
        )
        {
            this.healthCheckService = healthCheckService;
            this.logger = logger;
            this.startupTime = Process.GetCurrentProcess().StartTime;
        }

        [HttpGet]
        public async Task<IActionResult> GetServiceStatus()
        {
            var request = HttpContext.Request;
            logger.LogTrace($"{request.Method} {request.Path}");

            var report = await healthCheckService.CheckHealthAsync();
            logger.LogInformation(report.Status.ToString());

            var upTime = DateTime.Now - startupTime;
            var uptimeJson = upTime.ToString();

            var result = new SystemStatus
            {
                Uptime = uptimeJson,
                HealthInfo = report
            };

            var status = report.Status == HealthStatus.Healthy ?
                Ok(result) :
                StatusCode((int)HttpStatusCode.ServiceUnavailable, result);

            return status;
        }
    }
}
