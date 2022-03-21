using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using NXA.SC.Caas.Extensions;
using NXA.SC.Caas.Models;
using NXA.SC.Caas.Services.Mq;
using static NXA.SC.Caas.Services.CompilerBackgroundService;

namespace NXA.SC.Caas.Services.Persist.Impl
{
    public class TaskPersistService: ITaskPersistService
	{
        private readonly ILogger<TaskPersistService> logger;
		private readonly IMediator mediator;

		public TaskPersistService(
			ILogger<TaskPersistService> logger,
			IMediator mediator
		)
		{
			this.logger = logger;
			this.mediator = mediator;
        }

        public Task<CompilerTask[]> GetAll(int? offset, int? limit)
		{
            var command = new GetScheduledTasksCommand();
            var allTasks = mediator.Send(command);
            var result = allTasks.Result.Select(t => (CompilerTask)(t)).ToArray();
            return Task.FromResult(result);
        }

		public Task<CompilerTask?> GetByIdentifier(string identifier)
		{
            var command = new GetScheduledTasksCommand();
            var allTasks = mediator.Send(command);
            var result = allTasks.Result.SingleOrDefault(t => ((CompilerTask)(t)).Identifier == identifier) as CompilerTask;
            return Task.FromResult(result);
        }

        public Task<CompilerTask> Store(CreateCompilerTask task, bool asyncCompilation)
		{
            logger.LogDebug($"Storing: {task.ContractValues}...");
            var result = new CompilerTask(Guid.NewGuid().ToString(), CompilerTaskStatus.SCHEDULED, task, null, null);
			var mqCommand = new SendMqTaskCommand { Task = result };
			mediator.Send(mqCommand);

			if (asyncCompilation)
			{
				var command = new AddScheduledTaskCommand { Task = result };
				mediator.Send(command);
			}
			return Task.FromResult(result);
        }

        public Task<CompilerTask> Update(CompilerTask task, bool asyncCompilation) {
            logger.LogDebug($"Updating: {task.Create?.ContractValues}...");
			if(task.Result != null)
				task = CompilerTaskExtensions.SetStatus(task, CompilerTaskStatus.PROCESSED);
			else
				task = CompilerTaskExtensions.SetStatus(task, CompilerTaskStatus.FAILED);

			var mqCommand = new SendMqTaskCommand { Task = task };
			mediator.Send(mqCommand);
			if (asyncCompilation)
			{
				var command = new UpdateScheduledTaskCommand { Task = task };
				mediator.Send(command);
			}
			return Task.FromResult(task);
        }

		public Task<CompilerTask> DeleteByIdentifier(string identifier)
		{
			logger.LogDebug($"Deleting: {identifier}...");
			var command = new GetScheduledTasksCommand();
			var allTasks = mediator.Send(command);
			var taskToDelete = allTasks.Result.SingleOrDefault(t => ((CompilerTask)(t)).Identifier == identifier) as CompilerTask;

			if (taskToDelete == null)
			{
				throw new InvalidOperationException("Trying to delete nonexistent task");
			}
			var mqCommand = new SendMqTaskCommand { Task = taskToDelete };
			mediator.Send(mqCommand);
			var removeCommand = new RemoveScheduledTaskCommand { Identifier = identifier };
			mediator.Send(removeCommand);
			taskToDelete = CompilerTaskExtensions.SetStatus(taskToDelete, CompilerTaskStatus.DELETED);
			return Task.FromResult(taskToDelete!);
		}
	}

	public struct GetAllTasksCommand : IRequest<CompilerTask[]>
	{
		public int? Offset { get; set; }
		public int? Limit { get; set; }
	}

	public class GetAllTasksCommandHandler : IRequestHandler<GetAllTasksCommand, CompilerTask[]>
	{
		private readonly ITaskPersistService taskPersistService;

		public GetAllTasksCommandHandler(ITaskPersistService taskPersistService)
		{
			this.taskPersistService = taskPersistService;
		}

		public Task<CompilerTask[]> Handle(GetAllTasksCommand request, CancellationToken cancellationToken)
		{
			return taskPersistService.GetAll(request.Offset, request.Limit);
		}
	}

	public struct GetTasksByIdCommand : IRequest<CompilerTask>
	{
		public string Identifier { get; set; }
	}

	public class GetTasksByIdCommandHandler : IRequestHandler<GetTasksByIdCommand, CompilerTask?>
	{
		private readonly ITaskPersistService taskPersistService;
		
		public GetTasksByIdCommandHandler(ITaskPersistService taskPersistService)
		{
			this.taskPersistService = taskPersistService;
		}

		public Task<CompilerTask?> Handle(GetTasksByIdCommand request, CancellationToken cancellationToken)
		{
			return taskPersistService.GetByIdentifier(request.Identifier);
		}
	}

	public struct StoreTasksCommand : IRequest<CompilerTask>
	{
		public CreateCompilerTask Task { get; set; }
		public bool AsyncCompilation { get; set; }
	}

	public class StoreTasksCommandHandler : IRequestHandler<StoreTasksCommand, CompilerTask>
	{
		private readonly ITaskPersistService taskPersistService;

		public StoreTasksCommandHandler(ITaskPersistService taskPersistService)
		{
			this.taskPersistService = taskPersistService;
		}

		public Task<CompilerTask> Handle(StoreTasksCommand request, CancellationToken cancellationToken)
		{
			return taskPersistService.Store(request.Task, request.AsyncCompilation);
		}
	}

	public struct UpdateTasksCommand : IRequest<CompilerTask>
	{
		public CompilerTask Task { get; set; }
        public bool AsyncCompilation { get; set; }
    }

	public class UpdateTasksCommandHandler : IRequestHandler<UpdateTasksCommand, CompilerTask>
	{
		private readonly ITaskPersistService taskPersistService;

		public UpdateTasksCommandHandler(ITaskPersistService taskPersistService)
		{
			this.taskPersistService = taskPersistService;
		}

		public Task<CompilerTask> Handle(UpdateTasksCommand request, CancellationToken cancellationToken)
		{
			return taskPersistService.Update(request.Task, request.AsyncCompilation);
		}
	}

	public struct DeleteTasksByIdCommand : IRequest<CompilerTask>
	{
		public string Identifier { get; set; }
	}
	
	public class DeleteTasksByIdCommandHandler : IRequestHandler<DeleteTasksByIdCommand, CompilerTask>
	{
		private readonly ITaskPersistService taskPersistService;

		public DeleteTasksByIdCommandHandler(ITaskPersistService taskPersistService)
		{
			this.taskPersistService = taskPersistService;
		}

		public Task<CompilerTask> Handle(DeleteTasksByIdCommand request, CancellationToken cancellationToken)
		{
			return taskPersistService.DeleteByIdentifier(request.Identifier);
		}
	}
}
