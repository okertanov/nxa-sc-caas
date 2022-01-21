using NXA.SC.Caas.Models;

namespace NXA.SC.Caas.Services.Mq
{
	public interface IMqSettings
	{
		void SendCompilerTask(CompilerTask compilerTask);
	}
}