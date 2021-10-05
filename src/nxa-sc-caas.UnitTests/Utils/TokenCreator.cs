using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NXA.SC.Caas.Models;
using NXA.SC.Caas.Services.Token;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace nxa_sc_caas.UnitTests
{
    public class TokenCreator
    {
        private static ILogger<TokenService> _logger { get { return new LoggerFactory().CreateLogger<TokenService>(); } set { } }
        private static DbContextOptionsBuilder<ApiTokenContext> _dbOptBuilder { get { return new DbContextOptionsBuilder<ApiTokenContext>(); } set { } }
        public static async Task<ApiTokenContext> AddTokensToDb()
        {
            var db = new ApiTokenContext(_dbOptBuilder.Options);
            var testTokens = new List<ApiToken>{
                    new ApiToken()
                    {
                        Id = 1,
                        Guid = new Guid("b199ede5-107a-4cda-b475-7f55569daa05"),
                        Token= "b199ede5-107a-4cda-b475-7f55569daa05",
                        ExpirationDate = DateTime.Now.AddDays(1),
                        Active=true
                    },
                    new ApiToken()
                    {
                        Id = 2,
                        Guid = new Guid("2d713fbf-1c0b-4a55-b64e-d2d16905908e"),
                        Token= "2d713fbf-1c0b-4a55-b64e-d2d16905908e",
                        ExpirationDate = DateTime.Now.AddDays(-1),
                        Active=true
                    },
                    new ApiToken()
                    {
                        Id = 3,
                        Guid = new Guid("490b9e4e-66eb-4f8c-8166-c08d7056fdf0"),
                        Token= "490b9e4e-66eb-4f8c-8166-c08d7056fdf0",
                        ExpirationDate = DateTime.Now.AddDays(-1),
                        Active=false
                    }
                };
            db.Set<ApiToken>().AddRange(testTokens);
            await db.SaveChangesAsync();
            return db;
        }

        public static TokenService CreateTokenService(ApiTokenContext db)
        {
            var tokenService = new TokenService(_logger, db);
            return tokenService;
        }

        public static ApiTokenHandler CreateTokenHandler(ApiTokenContext db)
        {
            var options = new Mock<IOptionsMonitor<TokenAuthOptions>>();
            options
                .Setup(x => x.Get(It.IsAny<string>()))
                .Returns(new TokenAuthOptions());

            var logger = new Mock<ILogger<ApiTokenHandler>>();
            var loggerFactory = new Mock<ILoggerFactory>();
            loggerFactory.Setup(x => x.CreateLogger(It.IsAny<String>())).Returns(logger.Object);

            var encoder = new Mock<UrlEncoder>();
            var clock = new Mock<ISystemClock>();
            var tokenService = CreateTokenService(db);

            var tokenHandler = new ApiTokenHandler(options.Object, loggerFactory.Object, encoder.Object, clock.Object, tokenService);

            return tokenHandler;
        }


    }
}
