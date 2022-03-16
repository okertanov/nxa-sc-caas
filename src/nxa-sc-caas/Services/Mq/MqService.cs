using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
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

        private IConnection? connection;
        private IModel? channel;

        public MqService(ILogger<MqService> logger)
        {
            this.logger = logger;
            CreateConnection();
        }

        public void SendTask(IScheduledTask? task)
        {
            if (ConnectionExists())
            {
                var queueName = "CaasTasks";
                channel?.QueueDeclare(queueName, false, false, false, null);

                var json = JsonConvert.SerializeObject(task);
                var body = Encoding.UTF8.GetBytes(json);

                channel.BasicPublish("", queueName, null, body);
            }
            else
            {
                logger.LogError("RabbitMq connection doesn't exist");
            }
        }

        public void CreateConnection()
        {
            try
            {
                var factory = new ConnectionFactory
                {
                    //HostName = MqHost,
                    //UserName = MqUser,
                    //Password = MqPass
                    HostName = "localhost",
                    UserName = "rabbituser",
                    Password = "rabbitpass"
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
}
