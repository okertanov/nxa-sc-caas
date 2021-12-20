using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NXA.SC.Caas.Models;
using Microsoft.AspNetCore.Authorization;
using MediatR;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NXA.SC.Caas.Extensions;
using System.Text;
using System;
using NXA.SC.Caas.Services;

namespace NXA.SC.Caas.Controllers
{
    [ApiController]
    [Authorize(AuthenticationSchemes = TokenAuthOptions.DefaultScemeName)]
    [Route("[controller]")]
    [TypeFilter(typeof(PreprocessExceptionFilter))]
    public class TemplatePreprocessController : ControllerBase 
    {
        private readonly ILogger<StatusController> logger;
        private readonly IMediator mediator;
        private readonly string templateFolderPath = "../nxa-sc-caas.UnitTests/TestTokens";

        public TemplatePreprocessController(
            IMediator mediator,
            ILogger<StatusController> logger
        )
        {
            this.mediator = mediator;
            this.logger = logger;
        }

        [HttpGet("AvailableTemplates")]
        public IEnumerable<string> GetAvailableTemplates()
        {
            var request = HttpContext.Request;
            logger.LogTrace($"{request.Method} {request.Path}");

            var dirInfo = new DirectoryInfo(templateFolderPath);
            var files = dirInfo.GetFiles();
            var fileNames = files.Select(f => f.Name);
            return fileNames;
        }

        [HttpGet("TemplateSource")]
        public TemplateSource GetTemplateSource(string fileName)
        {
            var request = HttpContext.Request;
            logger.LogTrace($"{request.Method} {request.Path}");

            var sourceFilePath = Path.Combine(templateFolderPath, fileName);
            var sourceString = System.IO.File.ReadAllText(sourceFilePath);
            var sourceStrBytes = Encoding.UTF8.GetBytes(sourceString);
            var sourceBase64 = Convert.ToBase64String(sourceStrBytes);
            var preprocessCommand = new PreprocessTemplateCommand { SourceStr = sourceString, FileName = fileName };
            var templateParams = mediator.Send(preprocessCommand);
            var result = new TemplateSource { 
                SourceString = sourceString,
                SourceBase64 = sourceBase64,
                TemplateParams = templateParams.Result
            };
            return result;
        }

        [HttpGet("TemplateInputParamsFromTemplate")]
        public async Task<IEnumerable<TemplateParam>> GetTemplateParamsFromTemplate(string fileName)
        {
            var request = HttpContext.Request;
            logger.LogTrace($"{request.Method} {request.Path}");
            
            var sourceFilePath = Path.Combine(templateFolderPath, fileName);
            if (!System.IO.File.Exists(sourceFilePath))
            {
                logger.LogError("File not found");
                throw new FileNotFoundException("File not found");
            }
            var sourceStr = System.IO.File.ReadAllText(sourceFilePath);
            var preprocessCommand = new PreprocessTemplateCommand { SourceStr = sourceStr, FileName= Path.GetFileName(sourceFilePath) };
            return await mediator.Send(preprocessCommand);
        }

        [HttpGet("TemplateInputParamsFromSource")]
        public async Task<IEnumerable<TemplateParam>> GetTemplateParamsFromSource(string sourceCode)
        {
            var request = HttpContext.Request;
            logger.LogTrace($"{request.Method} {request.Path}");
            var sourceStrNormalized = sourceCode.IsBase64String() ? Encoding.UTF8.GetString(Convert.FromBase64String(sourceCode)) : sourceCode;

            var preprocessCommand = new PreprocessTemplateCommand { SourceStr = sourceStrNormalized, FileName= string.Empty };
            return await mediator.Send(preprocessCommand);
        }
    }
}
