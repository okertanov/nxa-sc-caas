using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using NXA.SC.Caas.Extensions;
using NXA.SC.Caas.Models;
using static NXA.SC.Caas.Models.CompilerBackgroundService;

namespace NXA.SC.Caas.Services.Persist.Impl {
    public class TaskPersistService: ITaskPersistService {
        private readonly ILogger<TaskPersistService> _logger;
		private readonly IMediator _mediator;

		public TaskPersistService(ILogger<TaskPersistService> logger, IMediator mediator)
		{
			_mediator = mediator;
			_logger = logger;
        }

        public Task<CompilerTask[]> GetAll(int? offset, int? limit) {
            var command = new GetScheduledTasksCommand();
            var allTasks = _mediator.Send(command);
            var result = allTasks.Result.Select(t => (CompilerTask)(t)).ToArray();
            return Task.FromResult(result);
        }

		public Task<CompilerTask?> GetByIdentifier(string identifier)
		{
            var command = new GetScheduledTasksCommand();
            var allTasks = _mediator.Send(command);
            var result = allTasks.Result.SingleOrDefault(t => ((CompilerTask)(t)).Identifier == identifier) as CompilerTask;
            return Task.FromResult(result);
        }

        public Task<CompilerTask> Store(CreateCompilerTask task, bool asyncCompilation) {
            _logger.LogDebug($"Storing: {task.ContractName}...");

            var result = new CompilerTask(Guid.NewGuid().ToString(), CompilerTaskStatus.SCHEDULED, task, null, null);
			if (asyncCompilation)
			{
				var command = new AddScheduledTaskCommand { Task = result };
				_mediator.Send(command);
			}
			return Task.FromResult(result);
        }

        public Task<CompilerTask> Update(CompilerTask task, bool asyncCompilation) {
            _logger.LogDebug($"Updating: {task.Create?.ContractName}...");
			if(task.Result != null)
				task = CompilerTaskExtensions.SetStatus(task, CompilerTaskStatus.PROCESSED);
			else
				task = CompilerTaskExtensions.SetStatus(task, CompilerTaskStatus.FAILED);
			if (asyncCompilation)
			{
				var command = new UpdateScheduledTaskCommand { Task = task };
				_mediator.Send(command);
			}
			return Task.FromResult(task);
        }

		public Task<CompilerTask?> DeleteByIdentifier(string identifier)
		{
			_logger.LogDebug($"Deleting: {identifier}...");
			var command = new GetScheduledTasksCommand();
			var allTasks = _mediator.Send(command);
			var taskToDelete = allTasks.Result.SingleOrDefault(t => ((CompilerTask)(t)).Identifier == identifier) as CompilerTask;

			if (taskToDelete != null)
			{
				var removeCommand = new RemoveScheduledTaskCommand { Identifier = identifier };
				_mediator.Send(removeCommand);
			}
			return Task.FromResult(taskToDelete);
		}
	}
	public class GetAllTasksCommand : IRequest<CompilerTask[]>
	{
		public int? Offset { get; set; }
		public int? Limit { get; set; }
	}
	public class GetAllTasksCommandHandler : IRequestHandler<GetAllTasksCommand, CompilerTask[]>
	{
		private readonly ITaskPersistService _taskPersistService;
		public GetAllTasksCommandHandler(ITaskPersistService taskPersistService)
		{
			_taskPersistService = taskPersistService;
		}

		public Task<CompilerTask[]> Handle(GetAllTasksCommand request, CancellationToken cancellationToken)
		{
			return _taskPersistService.GetAll(request.Offset, request.Limit);
		}
	}
	public class GetTasksByIdCommand : IRequest<CompilerTask>
	{
		public string Identifier { get; set; }
	}
	public class GetTasksByIdCommandHandler : IRequestHandler<GetTasksByIdCommand, CompilerTask?>
	{
		private readonly ITaskPersistService _taskPersistService;
		public GetTasksByIdCommandHandler(ITaskPersistService taskPersistService)
		{
			_taskPersistService = taskPersistService;
		}

		public Task<CompilerTask?> Handle(GetTasksByIdCommand request, CancellationToken cancellationToken)
		{
			return _taskPersistService.GetByIdentifier(request.Identifier);
		}
	}
	public class StoreTasksCommand : IRequest<CompilerTask>
	{
		public CreateCompilerTask Task { get; set; }
		public bool AsyncCompilation { get; set; }
	}
	public class StoreTasksCommandHandler : IRequestHandler<StoreTasksCommand, CompilerTask>
	{
		private readonly ITaskPersistService _taskPersistService;
		public StoreTasksCommandHandler(ITaskPersistService taskPersistService)
		{
			_taskPersistService = taskPersistService;
		}

		public Task<CompilerTask> Handle(StoreTasksCommand request, CancellationToken cancellationToken)
		{
			return _taskPersistService.Store(request.Task, request.AsyncCompilation);
		}
	}
	public class UpdateTasksCommand : IRequest<CompilerTask>
	{
		public CompilerTask Task { get; set; }
        public bool AsyncCompilation { get; set; }
    }
	public class UpdateTasksCommandHandler : IRequestHandler<UpdateTasksCommand, CompilerTask>
	{
		private readonly ITaskPersistService _taskPersistService;
		public UpdateTasksCommandHandler(ITaskPersistService taskPersistService)
		{
			_taskPersistService = taskPersistService;
		}

		public Task<CompilerTask> Handle(UpdateTasksCommand request, CancellationToken cancellationToken)
		{
			return _taskPersistService.Update(request.Task, request.AsyncCompilation);
		}
	}
	public class DeleteTasksByIdCommand : IRequest<CompilerTask>
	{
		public string Identifier { get; set; }
	}
	public class DeleteTasksByIdCommandHandler : IRequestHandler<DeleteTasksByIdCommand, CompilerTask>
	{
		private readonly ITaskPersistService _taskPersistService;
		public DeleteTasksByIdCommandHandler(ITaskPersistService taskPersistService)
		{
			_taskPersistService = taskPersistService;
		}

		public Task<CompilerTask> Handle(DeleteTasksByIdCommand request, CancellationToken cancellationToken)
		{
			return _taskPersistService.DeleteByIdentifier(request.Identifier);
		}
	}
}
