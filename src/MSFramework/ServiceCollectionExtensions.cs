using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using MSFramework.Audit;
using MSFramework.Domain;
using MSFramework.Domain.Event;
using MSFramework.Initializer;
using MSFramework.Reflection;

namespace MSFramework
{
	public static class ServiceCollectionExtensions
	{
		public static MSFrameworkBuilder UseEventDispatcher(this MSFrameworkBuilder builder, params Type[] eventTypes)
		{
			var excludeAssembly = typeof(MSFrameworkBuilder).Assembly;
			if (eventTypes.Any(x => x.Assembly != excludeAssembly))
			{
				var list = new List<Type>(eventTypes) {typeof(MSFrameworkBuilder)};
				builder.Services.AddEventDispatcher(list.ToArray());
			}
			else
			{
				builder.Services.AddEventDispatcher(eventTypes);
			}

			return builder;
		}

		public static void AddMSFramework(this IServiceCollection services,
			Action<MSFrameworkBuilder> builderAction = null)
		{
			var builder = new MSFrameworkBuilder(services);
			builderAction?.Invoke(builder);

			builder.UseInitializer();
			
			builder.Services.TryAddScoped<IUnitOfWorkManager, DefaultUnitOfWorkManager>();

			// 如果你想换成消息队列，则重新注册一个对应的服务即可
			builder.Services.TryAddScoped<IAuditService, DefaultAuditService>();

			var assemblies = AssemblyFinder.GetAllList();
			// todo: how to print logs in ConfigureService method
			Console.WriteLine($"Find assemblies: {string.Join(", ", assemblies.Select(x => x.GetName().Name))}");
		}

		public static void UseMSFramework(this IServiceProvider applicationServices)
		{
			InitializeAsync(applicationServices).GetAwaiter().GetResult();
		}

		private static async Task InitializeAsync(IServiceProvider applicationServices)
		{
			using var scope = applicationServices.CreateScope();
			var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("Initializer");
			var initializers = scope.ServiceProvider.GetServices<InitializerBase>().OrderBy(x => x.Order).ToList();
			logger.LogInformation($"{string.Join(", ", initializers.Select(x => x.GetType().FullName))}");
			foreach (var initializer in initializers)
			{
				await initializer.InitializeAsync(scope.ServiceProvider);
			}
		}
	}
}