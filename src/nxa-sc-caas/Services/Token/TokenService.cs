using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using NXA.SC.Caas.Models;

namespace NXA.SC.Caas.Services.Token
{
    public class TokenService : ITokenService
    {
        private readonly ILogger<TokenService> logger;
        private readonly ApiTokenContext context;

        public TokenService(ILogger<TokenService> logger, ApiTokenContext context) {
            this.logger = logger;
            this.context = context;
        }

        public bool TokenIsValid(string token)
        {
            var tokenActive = false;
            var tokenNotExpired = false;
            var valid = false;

            if (!context.Database.CanConnect())
            {
                logger.LogError("Cannot connect to the db!");
                return valid;
            }
            var tokenCount = context.Tokens.Count();

            if (tokenCount == 0)
            {
                logger.LogError("No tokens found in db!");
                return valid;
            }

            logger.LogInformation($"Total tokens found in in db: {tokenCount}");
            var tokenInDb = context.Tokens.SingleOrDefault(t => t.Token == token);
            var tokenExistsInDb = tokenInDb != null;

            if (tokenExistsInDb)
            {
                tokenActive = tokenInDb!.Active;
                tokenNotExpired = tokenInDb.ExpirationDate > DateTime.Now;
            }

            valid = tokenExistsInDb && tokenActive && tokenNotExpired;
            logger.LogInformation($"{(tokenInDb ?? new ApiToken()).Token} : " +
                $"Token exists in db = {tokenExistsInDb};" +
                $"Token is active = {tokenActive};" +
                $"Token is not expired = {tokenNotExpired}");

            return valid;
        }
    }

    public struct ValidateTokenCommand : IRequest<bool>
    {
        public string Token { get; set; }
    }

    public class TokenServiceCommandHandler : IRequestHandler<ValidateTokenCommand, bool>
    {
        private readonly ITokenService tokenService;
        
        public TokenServiceCommandHandler(ITokenService tokenService)
        {
            this.tokenService = tokenService;
        }

        public Task<bool> Handle(ValidateTokenCommand request, CancellationToken cancellationToken)
        {
            var isTokenValid = tokenService.TokenIsValid(request.Token);
            return Task.FromResult(isTokenValid);
        }
    }
}
