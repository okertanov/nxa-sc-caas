using System;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using System.Linq;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using NXA.SC.Caas.Services.Compiler.Impl;
using NXA.SC.Caas.Services.Persist.Impl;
using Microsoft.Extensions.Logging;
using NXA.SC.Caas.Models;

namespace NXA.SC.Caas.Services
{
    public class CompilerBackgroundService : HostedService
    {
        private static List<IScheduledTask> allTasks = new List<IScheduledTask>();
        public IServiceProvider serviceProvider;

        public CompilerBackgroundService(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public static IScheduledTask AddTask(IScheduledTask task)
        {
            allTasks.Add(task);
            return task;
        }

        public struct GetScheduledTasksCommand : IRequest<List<IScheduledTask>>
        {
        }

        public class GetScheduledTasksCommandHandler : IRequestHandler<GetScheduledTasksCommand, List<IScheduledTask>>
        {
            public Task<List<IScheduledTask>> Handle(GetScheduledTasksCommand request, CancellationToken cancellationToken)
            {
                return Task.FromResult(allTasks);
            }
        }

        public struct AddScheduledTaskCommand : IRequest<IScheduledTask>
        {
            public IScheduledTask Task { get; set; }
        }

        public class AddScheduledTaskCommandHandler : IRequestHandler<AddScheduledTaskCommand, IScheduledTask>
        {
            public Task<IScheduledTask> Handle(AddScheduledTaskCommand request, CancellationToken cancellationToken)
            {
                var task = AddTask(request.Task);
                return Task.FromResult(task);
            }
        }

        public struct UpdateScheduledTaskCommand : IRequest<IScheduledTask>
        {
            public IScheduledTask Task { get; set; }
        }

        public class UpdateScheduledTaskCommandHandler : IRequestHandler<UpdateScheduledTaskCommand, IScheduledTask>
        {
            public Task<IScheduledTask> Handle(UpdateScheduledTaskCommand request, CancellationToken cancellationToken)
            {
                var task = UpdateTask(request.Task);
                return Task.FromResult(task);
            }
        }

        public struct RemoveScheduledTaskCommand : IRequest<string>
        {
            public string Identifier { get; set; }
        }

        public class RemoveScheduledTaskCommandHandler : IRequestHandler<RemoveScheduledTaskCommand, string>
        {
            public Task<string> Handle(RemoveScheduledTaskCommand request, CancellationToken cancellationToken)
            {
                var task = RemoveTask(request.Identifier);
                return Task.FromResult(task);
            }
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            return Task.Run(async () => await BackgroundProcessing(stoppingToken));
        }

        private async Task BackgroundProcessing(CancellationToken cancellationToken)
        {
            var scope = serviceProvider.CreateScope();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<CompilerBackgroundService>>();
            var taskFactory = new TaskFactory(TaskScheduler.Current);
            var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

            var tasksRun = new List<Task>();

            while (!cancellationToken.IsCancellationRequested)
            {
                if (allTasks.Count == 0)
                    continue;

                var tasksThatShouldRun = allTasks.FindAll(t => t.Status == CompilerTaskStatus.SCHEDULED);

                foreach (var taskThatShouldRun in tasksThatShouldRun)
                {
                    tasksRun.Add(
                        taskFactory.StartNew(
                            async () =>
                            {
                                logger.LogInformation($"Processing task id: {taskThatShouldRun.Identifier}");

                                var compileCommand = new CompileCommand { Task = (taskThatShouldRun as CompilerTask)! };
                                var compiled = await mediator.Send(compileCommand);

                                var updateCommand = new UpdateTasksCommand { Task = compiled };
                                var updated = await mediator.Send(updateCommand);
                                
                                logger.LogInformation($"Task {taskThatShouldRun.Identifier} finished");
                            },
                        cancellationToken)
                    );
                }

                await Task.WhenAll(tasksRun);

                // TODO: Introduce delay to not stress by 'FindAll'
            }
        }

        private static IScheduledTask UpdateTask(IScheduledTask updatedTask)
        {
            RemoveTask(updatedTask.Identifier);
            allTasks.Add(updatedTask);
            return allTasks.Single(t => t.Identifier == updatedTask.Identifier);
        }

        private static string RemoveTask(string identifier)
        {
            allTasks.RemoveAll(t => t.Identifier == identifier);
            return identifier;
        }
    }
}
