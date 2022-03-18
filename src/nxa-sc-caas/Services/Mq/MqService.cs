using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NXA.SC.Caas.Models;
using RabbitMQ.Client;

namespace NXA.SC.Caas.Services.Mq
{
    public class MqService : IMqService
    {
        private readonly ILogger<MqService> logger;

        public string? MqHost => Environment.GetEnvironmentVariable("RABBITMQ_HOST");
        public string? MqUser => Environment.GetEnvironmentVariable("RABBITMQ_DEFAULT_USER");
        public string? MqPass => Environment.GetEnvironmentVariable("RABBITMQ_DEFAULT_PASS");
        private string queueName => "CaasTasks";

        private IConnection? connection;
        private IModel? channel;

        public MqService(ILogger<MqService> logger)
        {
            this.logger = logger;
            CreateConnection();
        }

        public string SendTask(IScheduledTask? task)
        {
            var json = JsonConvert.SerializeObject(task);
            var body = Encoding.UTF8.GetBytes(json);

            if (ConnectionExists())
            {
                channel?.QueueDeclare(queueName, false, false, false, null);
                channel.BasicPublish("", queueName, null, body);
            }
            else
            {
                logger.LogError("RabbitMq connection doesn't exist");
            }

            return json;
        }

        public void CreateConnection()
        {
            try
            {
                var factory = new ConnectionFactory
                {
                    HostName = MqHost,
                    UserName = MqUser,
                    Password = MqPass
                };
                connection = factory.CreateConnection();
                channel = connection?.CreateModel();
            }
            catch (Exception ex)
            {
                logger.LogError($"{ex.Message}, {ex.StackTrace}");
            }
        }

        private bool ConnectionExists()
        {
            if (connection != null)
            {
                return true;
            }

            CreateConnection();
            return connection != null;
        }

    }

    public struct SendMqTaskCommand : IRequest<string>
    {
        public IScheduledTask Task { get; set; }
    }

    public class SendMqTaskCommandHandler : IRequestHandler<SendMqTaskCommand, string>
    {
        private readonly IMqService mqService;

        public SendMqTaskCommandHandler(IMqService mqService)
        {
            this.mqService = mqService;
        }

        public Task<string> Handle(SendMqTaskCommand request, CancellationToken cancellationToken)
        {
            return Task.FromResult(mqService.SendTask(request.Task));
        }
    }
}
