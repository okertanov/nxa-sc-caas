using System;
using System.Linq;
using System.Text;
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
            var tokenInDb = _context.Tokens.SingleOrDefault(t=>t.Token == token);
            bool tokenExistsInDb = tokenInDb != null;

            if (tokenExistsInDb)
            {
                tokenActive = tokenInDb!.Active;
                tokenNotExpired = tokenInDb.ExpirationDate > DateTime.Now;
            }

            var valid = tokenExistsInDb && tokenActive && tokenNotExpired;
            _logger.LogInformation($"{(tokenInDb ?? new ApiToken()).Token} : " +
                $"Token exists in db = {tokenExistsInDb};" +
                $"Token is active = {tokenActive};" +
                $"Token is not expired = {tokenNotExpired}");

            return valid;
        }
    }
}
