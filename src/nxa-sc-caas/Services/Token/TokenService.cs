using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NXA.SC.Caas.Models;

namespace NXA.SC.Caas.Services.Token {
    public class TokenService : ITokenService
    {
        private readonly ILogger<TokenService> _logger;
        private readonly ApiTokenContext _context;

        public TokenService(ILogger<TokenService> logger, ApiTokenContext context) {
            _logger = logger;
            _context = context;
        }
        public bool TokenIsValid(string token)
        {
            bool tokenActive = false;
            bool tokenNotExpired = false;
            bool valid = false;

            if (!_context.Database.CanConnect())
            {
                _logger.LogError("Cannot connect to the db!");
                return valid;
            }
            var tokenCount = _context.Tokens.Count();

            if (tokenCount == 0)
            {
                _logger.LogError("No tokens found in db!");
                return valid;
            }

            _logger.LogInformation($"Total tokens found in in db: {tokenCount}");
            var tokenInDb = _context.Tokens.SingleOrDefault(t => t.Token == token);
            bool tokenExistsInDb = tokenInDb != null;

            if (tokenExistsInDb)
            {
                tokenActive = tokenInDb!.Active;
                tokenNotExpired = tokenInDb.ExpirationDate > DateTime.Now;
            }

            valid = tokenExistsInDb && tokenActive && tokenNotExpired;
            _logger.LogInformation($"{(tokenInDb ?? new ApiToken()).Token} : " +
                $"Token exists in db = {tokenExistsInDb};" +
                $"Token is active = {tokenActive};" +
                $"Token is not expired = {tokenNotExpired}");

            return valid;
        }
    }
    public class ValidateTokenCommand : IRequest<bool>
    {
        public string Token { get; set; }
    }
    public class TokenServiceCommandHandler : IRequestHandler<ValidateTokenCommand, bool>
    {
        private readonly ITokenService _tokenService;
        public TokenServiceCommandHandler(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }
        public async Task<bool> Handle(ValidateTokenCommand request, CancellationToken cancellationToken)
        {
            return _tokenService.TokenIsValid(request.Token);
        }
    }
}
