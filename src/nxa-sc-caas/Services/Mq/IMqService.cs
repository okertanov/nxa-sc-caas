﻿using NXA.SC.Caas.Models;

namespace NXA.SC.Caas.Services.Mq
{
	public interface IMqService
	{
		void SendTask(IScheduledTask? task);
	}
}