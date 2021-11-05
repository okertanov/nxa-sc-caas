using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;

namespace NXA.SC.Caas.Services.Db
{
    public class DbSettings : IDbSettings
    {
        private readonly ILogger<DbSettings> logger;
        public string? DbHost => Environment.GetEnvironmentVariable("DB_HOST");
        public string? DbPort => Environment.GetEnvironmentVariable("DB_PORT");
        public string? DbUser => Environment.GetEnvironmentVariable("API_DB_USER");
        public string? DbPass => Environment.GetEnvironmentVariable("API_DB_PASS");

        public DbSettings(ILogger<DbSettings> logger)
        {
            this.logger = logger;
        }

        public string GetConnectionString()
        {
            var connVals = new { DbHost, DbPort, DbUser, DbPass };
            logger.LogInformation("Db connection params: {@connVals}", connVals);
            return $"Host={DbHost};Port={DbPort};Username={DbUser};Password={DbPass};Database=caas_database;";
        }
    }

    public struct GetConnStrCommand : IRequest<string>
    {
    }

    public class GetConnStrCommandHandler : IRequestHandler<GetConnStrCommand, string>
    {
        private readonly IDbSettings _dbSettings;
        public GetConnStrCommandHandler(IDbSettings dbSettings)
        {
            _dbSettings = dbSettings;
        }

        public Task<string> Handle(GetConnStrCommand request, CancellationToken cancellationToken)
        {
            var connectionString = _dbSettings.GetConnectionString();
            return Task.FromResult(connectionString);
        }
    }
}
