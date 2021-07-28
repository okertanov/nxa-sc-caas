using NXA.SC.Caas.Models;

namespace NXA.SC.Caas.Services.Token
{
	public interface ITokenService
	{
		string GenerateWebToken();
	}
}