using dotenv.net;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NXA.SC.Caas.Services.Db;
using System;
using System.IO;

namespace NXA.SC.Caas.Models
{
    public class ApiTokenContext : DbContext
    {
        private readonly IMediator _mediator;
        public ApiTokenContext(IMediator mediator)
        {
            _mediator = mediator;
        }
        public DbSet<ApiToken> Tokens { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            var command = new GetConnStrCommand();
            var connStr= _mediator.Send(command).Result;
            options.UseNpgsql(connStr);
        }
    }
}
