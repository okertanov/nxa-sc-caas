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

        public static readonly string? DbHost = Environment.GetEnvironmentVariable("API_DB_HOST");
        public static readonly string? DbPort = Environment.GetEnvironmentVariable("API_DB_PORT");
        public static readonly string? DbUser = Environment.GetEnvironmentVariable("API_DB_USER");
        public static readonly string? DbPass = Environment.GetEnvironmentVariable("API_DB_PASSWORD");
        public static readonly string? DbDtataBase = Environment.GetEnvironmentVariable("API_DB_DATABASE");

        public DbSettings(ILogger<DbSettings> logger)
        {
            this.logger = logger;
            var connVals = new { DbHost, DbPort, DbUser, DbPass, DbDtataBase };
            logger.LogInformation("Initializing with DB connection params: {@connVals}", connVals);
        }

        public string GetConnectionString()
        {
            var connStr = $"Host={DbHost};Port={DbPort};Username={DbUser};Password={DbPass};Database={DbDtataBase};";
            logger.LogInformation("DB connection params: '{@connStr}'", connStr);
            return connStr;
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
