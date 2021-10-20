using dotenv.net;
using Microsoft.EntityFrameworkCore;
using NXA.SC.Caas.Services.Db;
using System;
using System.IO;

namespace NXA.SC.Caas.Models
{
    public class ApiTokenContext : DbContext
    {
        private readonly IDbSettings _tokenSettings;
        public ApiTokenContext(IDbSettings tokenSettings)
        {
            _tokenSettings = tokenSettings;
        }
        public DbSet<ApiToken> Tokens { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            var connStr= _tokenSettings.GetConnectionString();
            options.UseNpgsql(connStr);
        }
    }
}
