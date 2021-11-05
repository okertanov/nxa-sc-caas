using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using NXA.SC.Caas.Services.Token;
using System.Security.Principal;
using MediatR;

namespace NXA.SC.Caas.Models
{
    public class TokenAuthOptions : AuthenticationSchemeOptions
    {
        public const string DefaultScemeName = "TokenAuthScheme";
    }

    public class ApiTokenHandler : AuthenticationHandler<TokenAuthOptions>
    {
        private readonly IMediator mediator;
        
        public ApiTokenHandler(
            IOptionsMonitor<TokenAuthOptions> options,
            ILoggerFactory logger,
            UrlEncoder encoder,
            ISystemClock clock,
            IMediator mediator
        ) : base(options, logger, encoder, clock)
        {
            this.mediator = mediator;
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var token = Request.Headers["Token"].ToString();
            var command = new ValidateTokenCommand { Token = token };
            var valid = mediator.Send(command).Result;
            if (valid)
            {
                var identity = new GenericIdentity("id");
                var principal = new ClaimsPrincipal(identity);
                var ticket = new AuthenticationTicket(principal, Scheme.Name);
                return Task.FromResult(AuthenticateResult.Success(ticket));
            }
            return Task.FromResult(AuthenticateResult.Fail("Token not found in database"));
        }
    }
}
