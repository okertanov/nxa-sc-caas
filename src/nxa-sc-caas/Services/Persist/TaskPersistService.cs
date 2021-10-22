using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using NXA.SC.Caas.Models;

namespace NXA.SC.Caas.Services.Persist.Impl {
    public class TaskPersistService: ITaskPersistService {
        private readonly ILogger<TaskPersistService> _logger;

        public TaskPersistService(ILogger<TaskPersistService> logger) {
            _logger = logger;
        }

        public Task<CompilerTask[]> GetAll(int? offset, int? limit) {
            var result = new [] { new CompilerTask() };
            return Task.FromResult(result);
        }

        public Task<CompilerTask> GetByIdentifier(string identifier) {
            var result = new CompilerTask();
            return Task.FromResult(result);
        }

        public Task<CompilerTask> Store(CreateCompilerTask task) {
            _logger.LogDebug($"Storing: {task.ContractName}...");

            var result = new CompilerTask("", CompilerTaskStatus.CREATED, task, null, null);
            return Task.FromResult(result);
        }

        public Task<CompilerTask> Update(CompilerTask task) {
            _logger.LogDebug($"Updating: {task.Create?.ContractName}...");

            return Task.FromResult(task);
        }

        public Task<CompilerTask> DeleteByIdentifier(string identifier) {
            var result = new CompilerTask();
            return Task.FromResult(result);
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
	public class GetTasksByIdCommandHandler : IRequestHandler<GetTasksByIdCommand, CompilerTask>
	{
		private readonly ITaskPersistService _taskPersistService;
		public GetTasksByIdCommandHandler(ITaskPersistService taskPersistService)
		{
			_taskPersistService = taskPersistService;
		}

		public Task<CompilerTask> Handle(GetTasksByIdCommand request, CancellationToken cancellationToken)
		{
			return _taskPersistService.GetByIdentifier(request.Identifier);
		}
	}
	public class StoreTasksCommand : IRequest<CompilerTask>
	{
		public CreateCompilerTask Task { get; set; }
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
			return _taskPersistService.Store(request.Task);
		}
	}
	public class UpdateTasksCommand : IRequest<CompilerTask>
	{
		public CompilerTask Task { get; set; }
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
			return _taskPersistService.Update(request.Task);
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
