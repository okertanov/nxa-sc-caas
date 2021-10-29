using System;
using System.Runtime.Serialization;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;
using System.Threading;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace NXA.SC.Caas.Models {
    public interface IScheduledTask
    {
        string Identifier { get; }
        CompilerTaskStatus Status { get; }
    }
}
