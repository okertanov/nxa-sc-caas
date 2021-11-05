using System;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace NXA.SC.Caas.Models
{
    public class SystemStatus
    {
        public string Uptime { get; set; } = String.Empty;
        public HealthReport HealthInfo { get; set; } = default!;
  
    }
}
