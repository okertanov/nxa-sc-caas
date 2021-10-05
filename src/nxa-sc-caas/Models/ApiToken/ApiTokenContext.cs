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
            var envFilePath= Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), @"..\..\"));
            var dotEnvOpts = new DotEnvOptions(ignoreExceptions: false, envFilePaths: new[] { Path.Combine(envFilePath, ".env") });
            DotEnv.Load(dotEnvOpts);
            var allEnvVals = DotEnv.Read(dotEnvOpts);
            var host = allEnvVals["DB_HOST"];
            var port = allEnvVals["DB_PORT"];
            var connString = $"Host={host};Port={port};Username=docker;Password=docker;Database=caas_database;";
            options.UseNpgsql(connString);
        }
    }
}
