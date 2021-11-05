using MediatR;
using Microsoft.EntityFrameworkCore;
using NXA.SC.Caas.Services.Db;

namespace NXA.SC.Caas.Models
{
    public class ApiTokenContext : DbContext
    {
        private readonly IMediator _mediator;

        public DbSet<ApiToken> Tokens { get; set; } = default!;

        public ApiTokenContext(IMediator mediator)
        {
            _mediator = mediator;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            var command = new GetConnStrCommand();
            var connStr= _mediator.Send(command).Result;

            if (connStr != null)
            {
                options.UseNpgsql(connStr);
            }
            else
            {
                options.UseInMemoryDatabase("Tokens");
            }
        }
    }
}
