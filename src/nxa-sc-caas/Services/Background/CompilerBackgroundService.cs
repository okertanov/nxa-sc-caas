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

namespace NXA.SC.Caas.Models {
    public class CompilerBackgroundService : HostedService
    {
        private static List<IScheduledTask> _allTasks = new List<IScheduledTask>();
        public IServiceProvider _serviceProvider;
        public CompilerBackgroundService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public static IScheduledTask AddTask(IScheduledTask task)
        {
            _allTasks.Add(task);
            return task;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _ = Task.Run(async () => await BackgroundProcessing(stoppingToken));
        }
        private async Task BackgroundProcessing(CancellationToken cancellationToken)
        {
            var tasksRun = new List<Task>();

            while (!cancellationToken.IsCancellationRequested)
            {
                var taskFactory = new TaskFactory(TaskScheduler.Current);

                using var scope = _serviceProvider.CreateScope();
                var _logger =
                scope.ServiceProvider
                    .GetRequiredService<ILogger<CompilerBackgroundService>>();

                if (_allTasks.Count == 0)
                    continue;

                var tasksThatShouldRun = _allTasks.FindAll(t => t.Status == CompilerTaskStatus.SCHEDULED);

                foreach (var taskThatShouldRun in tasksThatShouldRun)
                {
                    tasksRun.Add(taskFactory.StartNew(
                    async () =>
                    {
                        _logger.LogInformation($"Processing task id: {taskThatShouldRun.Identifier}");
                        var _mediator =
                            scope.ServiceProvider
                                .GetRequiredService<IMediator>();

                        var compileCommand = new CompileCommand { Task = taskThatShouldRun as CompilerTask };
                        var compiled = await _mediator.Send(compileCommand);

                        var updateCommand = new UpdateTasksCommand { Task = compiled };
                        var updated = await _mediator.Send(updateCommand);
                        _logger.LogInformation($"Task {taskThatShouldRun.Identifier} finished");
                    },
                    cancellationToken));
                }

                await Task.WhenAll(tasksRun);
            }
        }
        private static IScheduledTask UpdateTask(IScheduledTask updatedTask)
        {
            RemoveTask(updatedTask.Identifier);
            _allTasks.Add(updatedTask);

            return _allTasks.Single(t => t.Identifier == updatedTask.Identifier);
        }

        private static string RemoveTask(string identifier)
        {
            _allTasks.
                RemoveAll(t => t.Identifier == identifier);

            return identifier;
        }
        public class GetScheduledTasksCommand : IRequest<List<IScheduledTask>> { }
        public class GetScheduledTasksCommandHandler : IRequestHandler<GetScheduledTasksCommand, List<IScheduledTask>>
        {
            public async Task<List<IScheduledTask>> Handle(GetScheduledTasksCommand request, CancellationToken cancellationToken)
            {
                return _allTasks;
            }
        }
        public class AddScheduledTaskCommand : IRequest<IScheduledTask> {
            public IScheduledTask Task { get; set; }
        }
        public class AddScheduledTaskCommandHandler : IRequestHandler<AddScheduledTaskCommand, IScheduledTask>
        {
            public async Task<IScheduledTask> Handle(AddScheduledTaskCommand request, CancellationToken cancellationToken)
            {
                return AddTask(request.Task);
            }
        }
        public class UpdateScheduledTaskCommand : IRequest<IScheduledTask>
        {
            public IScheduledTask Task { get; set; }
        }
        public class UpdateScheduledTaskCommandHandler : IRequestHandler<UpdateScheduledTaskCommand, IScheduledTask>
        {
            public async Task<IScheduledTask> Handle(UpdateScheduledTaskCommand request, CancellationToken cancellationToken)
            {
                return UpdateTask(request.Task);
            }
        }
        public class RemoveScheduledTaskCommand : IRequest<string>
        {
            public string Identifier { get; set; } = string.Empty;
        }
        public class RemoveScheduledTaskCommandHandler : IRequestHandler<RemoveScheduledTaskCommand, string>
        {
            public async Task<string> Handle(RemoveScheduledTaskCommand request, CancellationToken cancellationToken)
            {
                return RemoveTask(request.Identifier);
            }
        }
    }
}
