using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MSFramework.Domain;
using MSFramework.Initializer;

namespace MSFramework.Function
{
	public class FunctionInitializer : InitializerBase
	{
		public override async Task InitializeAsync(IServiceProvider serviceProvider)
		{
			var functionFinder = serviceProvider.GetService<IFunctionFinder>();
			var logger = serviceProvider.GetRequiredService<ILogger<FunctionInitializer>>();
			if (functionFinder == null)
			{
				logger.LogInformation("没有配置 Function 中间件");
				return;
			}

			var functionsInApp = functionFinder.GetAllList();

			var functionsInAppDict = new Dictionary<string, FunctionDefine>();
			foreach (var function in functionsInApp)
			{
				if (!functionsInAppDict.ContainsKey(function.Code))
				{
					functionsInAppDict.Add(function.Code, function);
				}
				else
				{
					throw new MSFrameworkException($"There are same functions: {function.Code}");
				}
			}

			var repository = serviceProvider.GetService<IFunctionRepository>();
			var functionsInDatabaseDict = repository.GetAllList()
				.ToDictionary(x => x.Code, x => x);

			// 添加新功能
			foreach (var kv in functionsInAppDict)
			{
				var function = kv.Value;
				if (!functionsInDatabaseDict.ContainsKey(function.Code))
				{
					function.SetCreationAudited("System", "System");
					await repository.InsertAsync(function);
				}
				else
				{
					if (function.Expired)
					{
						function.Renewal();
						function.SetModificationAudited("System", "System");
						await repository.UpdateAsync(function);
					}
				}
			}

			// 标记功能过期
			foreach (var kv in functionsInDatabaseDict)
			{
				var function = kv.Value;
				if (!functionsInAppDict.ContainsKey(kv.Key))
				{
					function.Expire();
					function.SetModificationAudited("System", "System");
					await repository.UpdateAsync(function);
				}
			}

			await serviceProvider.GetRequiredService<IUnitOfWorkManager>().CommitAsync();
		}
	}
}