using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NXA.SC.Caas.Extensions;
using NXA.SC.Caas.Models;
using NXA.SC.Caas.Shared.Utils;

namespace NXA.SC.Caas.Services.Compiler.Impl
{
    public class CompilerTaskService : ICompilerTaskService
    {
        private readonly ILogger<CompilerTaskService> logger;
        private readonly IMediator mediator;

        public CompilerTaskService(ILogger<CompilerTaskService> logger, IMediator mediator)
        {
            this.logger = logger;
            this.mediator = mediator;
        }
        public CompilerTask ModifyCompilerTask(CompilerTask task)
        {
            var resultTask = task;


            return resultTask;
        }
    }
}
