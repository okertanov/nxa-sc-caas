using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace NXA.SC.Caas.Services.Token {
    public class TokenService : ITokenService
    {
        private readonly ILogger<TokenService> _logger;
        private readonly IConfiguration _config;   

        public TokenService(ILogger<TokenService> logger, IConfiguration config) {
            _logger = logger;
            _config = config;
        }
        public string GenerateWebToken()
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var issuer = _config["Jwt:Issuer"];
            var jwtValidity = DateTime.Now.AddMinutes(Convert.ToDouble(_config["Jwt:TokenExpiry"]));

            var token = new JwtSecurityToken(
              issuer,
              null,
              expires: jwtValidity,
              signingCredentials: creds);

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            _logger.LogInformation($"Token generated: {tokenString}");
            return tokenString;
        }
    }
}
