using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Text.Encodings.Web;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using System.Linq;
using NXA.SC.Caas.Services.Token;
using System.Security.Principal;

namespace NXA.SC.Caas.Models {
    public class TokenAuthOptions : AuthenticationSchemeOptions
    {
        public const string DefaultScemeName = "TokenAuthScheme";
    }

    public class ApiTokenHandler : AuthenticationHandler<TokenAuthOptions>
    {
        private readonly ITokenService _tokenService;
        public ApiTokenHandler(IOptionsMonitor<TokenAuthOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock, ITokenService tokenService)
    : base(options, logger, encoder, clock)
        {
            _tokenService = tokenService;
        }
        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var token = Request.Headers["Token"].ToString();

            if (_tokenService.TokenIsValid(token))
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
