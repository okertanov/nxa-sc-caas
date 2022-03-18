using NXA.SC.Caas.Models;

namespace NXA.SC.Caas.Services.Mq
{
	public interface IMqService
	{
		string SendTask(IScheduledTask? task);
		void CreateConnection();
	}
}