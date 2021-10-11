using dotenv.net;
using Microsoft.EntityFrameworkCore;
using System;
using System.IO;

namespace NXA.SC.Caas.Models
{
    public class ApiTokenContext : DbContext
    {
        public ApiTokenContext(DbContextOptions<ApiTokenContext> options)
            : base(options)
        {

        }
        public DbSet<ApiToken> Tokens { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            var dbHost = Environment.GetEnvironmentVariable("DB_HOST");
            var dbPort = Environment.GetEnvironmentVariable("DB_PORT");
            var dbUser = Environment.GetEnvironmentVariable("API_DB_USER");
            var dbPwd = Environment.GetEnvironmentVariable("API_DB_PASS");
            var connString = $"Host={dbHost};Port={dbPort};Username={dbUser};Password={dbPwd};Database=caas_database;";
            options.UseNpgsql(connString);
        }
    }
}
