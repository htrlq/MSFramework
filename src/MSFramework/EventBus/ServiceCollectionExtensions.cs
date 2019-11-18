using System;
using Microsoft.Extensions.DependencyInjection;

namespace MSFramework.EventBus
{
	public static class ServiceCollectionExtensions
	{
		public static MSFrameworkBuilder UsePassThroughEventBus(this MSFrameworkBuilder builder,
			Action<EventBusBuilder> configure = null)
		{
			EventBusBuilder eBuilder = new EventBusBuilder(builder.Services);
			configure?.Invoke(eBuilder);

			builder.Services.AddPassThroughEventBus();

			return builder;
		}

		public static IServiceCollection AddPassThroughEventBus(this IServiceCollection services)
		{			
			services.AddSingleton<IEventBus, PassThroughEventBus>();
			return services;
		}
	}
}