using Microsoft.EntityFrameworkCore;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;
using System;

namespace NXA.SC.Caas.Services.Db
{
	public interface IDbSettings
	{
        string GetConnectionString();
	}
}