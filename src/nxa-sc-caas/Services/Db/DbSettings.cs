using System;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NXA.SC.Caas.Models;

namespace NXA.SC.Caas.Services.Db {
    public class DbSettings : IDbSettings
    {
        private readonly ILogger<DbSettings> _logger;
        public string? DbHost => Environment.GetEnvironmentVariable("DB_HOST");
        public string? DbPort => Environment.GetEnvironmentVariable("DB_PORT");
        public string? DbUser => Environment.GetEnvironmentVariable("API_DB_USER");
        public string? DbPass => Environment.GetEnvironmentVariable("API_DB_PASS");


        public DbSettings(ILogger<DbSettings> logger) {
            _logger = logger;
        }

        public string GetConnectionString()
        {
            var connVals = new { DbHost, DbPort, DbUser, DbPass };
            _logger.LogInformation("Db connection params: {@connVals}", connVals);
            return $"Host={DbHost};Port={DbPort};Username={DbUser};Password={DbPass};Database=caas_database;";
        }
    }
}
