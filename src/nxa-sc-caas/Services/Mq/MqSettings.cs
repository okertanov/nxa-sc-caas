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
    public class MqSettings : IMqSettings
    {
        private readonly ILogger<MqSettings> logger;
        public string? MqHost => Environment.GetEnvironmentVariable("RABBITMQ_HOST");
        public string? MqUser => Environment.GetEnvironmentVariable("RABBITMQ_DEFAULT_USER");
        public string? MqPass => Environment.GetEnvironmentVariable("RABBITMQ_DEFAULT_PASS");

        private IConnection connection;

        public MqSettings(ILogger<MqSettings> logger)
        {
            this.logger = logger;
            CreateConnection();
        }

        public void SendCompilerTask(CompilerTask compilerTask)
        {
            if (ConnectionExists())
            {
                using (var channel = connection.CreateModel())
                {
                    var queueName = "CompilerTasks";
                    channel.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);

                    var json = JsonConvert.SerializeObject(compilerTask);
                    var body = Encoding.UTF8.GetBytes(json);

                    channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: null, body: body);
                }
            }
        }

        private void CreateConnection()
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
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Could not create connection: {ex.Message}");
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
