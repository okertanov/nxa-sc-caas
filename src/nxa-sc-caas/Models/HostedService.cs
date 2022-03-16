using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NXA.SC.Caas.Services.Mq;

namespace NXA.SC.Caas.Models
{
    public abstract class HostedService : IHostedService
    {
        private Task executingTask = default!;
        private CancellationTokenSource cts = default!;
        public IServiceProvider serviceProvider;

        public Task StartAsync(CancellationToken cancellationToken)
        {
            var scope = serviceProvider.CreateScope();
            var mqService = scope.ServiceProvider.GetRequiredService<IMqService>();
            mqService.CreateConnection();
            cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            executingTask = ExecuteAsync(cts.Token);
            return executingTask.IsCompleted ? executingTask : Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            if (executingTask == null)
            {
                return;
            }

            cts.Cancel();
            await Task.WhenAny(executingTask, Task.Delay(-1, cancellationToken));

            cancellationToken.ThrowIfCancellationRequested();
        }

        protected abstract Task ExecuteAsync(CancellationToken cancellationToken);
    }
}
