using System;
using System.Collections.Generic;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace NXA.SC.Caas.Models
{
    public class TemplateSource
    {
        public string SourceString { get; set; } = String.Empty;
        public string SourceBase64 { get; set; } = String.Empty;
        public IEnumerable<TemplateParam> TemplateParams { get; set; } = default!;
  
    }
}
