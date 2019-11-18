using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MSFramework.Collections.Generic;
using MSFramework.Common;

namespace MSFramework.EntityFrameworkCore
{
	public class EntityFrameworkInitializer : IInitializer
	{
		private readonly IServiceProvider _serviceProvider;

		public EntityFrameworkInitializer(IServiceProvider serviceProvider)
		{
			_serviceProvider = serviceProvider;
		}

		public void Initialize()
		{
			using var scope = _serviceProvider.CreateScope();
			var dbContextFactory = scope.ServiceProvider.GetRequiredService<DbContextFactory>();
			foreach (var kv in EntityFrameworkOptions.EntityFrameworkOptionDict)
			{
				var useTrans = kv.Value.UseTransaction;
				if (kv.Value.AutoMigrationEnabled)
				{
					kv.Value.UseTransaction = false;
					var dbContext = dbContextFactory.Create(kv.Value);
					if (dbContext == null)
					{
						continue;
					}

					if (dbContext.Database.ProviderName == "Microsoft.EntityFrameworkCore.InMemory") continue;
					
					var migrations = dbContext.Database.GetPendingMigrations().ToArray();
					if (migrations.Length > 0)
					{
						dbContext.Database.Migrate();
						ILogger logger = dbContext.GetService<ILoggerFactory>()
							.CreateLogger<EntityFrameworkInitializer>();
						logger.LogInformation($"已提交{migrations.Length}条挂起的迁移记录：{migrations.ExpandAndToString()}");
					}

					kv.Value.UseTransaction = useTrans;
				}
			}
		}
	}
}