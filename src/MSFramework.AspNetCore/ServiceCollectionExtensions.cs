using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MSFramework.AspNetCore.Function;
using MSFramework.Function;
using ISession = MSFramework.Domain.ISession;

namespace MSFramework.AspNetCore
{
	public static class ServiceCollectionExtensions
	{
		public static MSFrameworkBuilder UseAspNetCore(this MSFrameworkBuilder builder)
		{
			builder.Services.TryAddSingleton<IFunctionFinder, AspNetCoreFunctionFinder>();
			builder.Services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
			builder.Services.TryAddSingleton<ISession, AspNetCoreSession>();
			builder.Services.AddSingleton<IActionResultTypeMapper, ActionResultTypeMapper>();
			return builder;
		}

		public static void UseMSFramework(this IApplicationBuilder builder)
		{
			builder.ApplicationServices.UseMSFramework();
		}
	}
}