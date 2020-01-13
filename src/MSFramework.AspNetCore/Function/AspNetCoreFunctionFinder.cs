using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using MSFramework.AspNetCore.Extensions;
using MSFramework.Function;

namespace MSFramework.AspNetCore.Function
{
	public class AspNetCoreFunctionFinder : IFunctionFinder
	{
		private readonly IServiceProvider _services;

		public AspNetCoreFunctionFinder(IServiceProvider serviceProvider)
		{
			_services = serviceProvider;
		}

		public List<MSFramework.Function.Function> GetAllList()
		{
			var actionDescriptorCollectionProvider =
				_services.GetRequiredService<IActionDescriptorCollectionProvider>();
			var functions = new List<MSFramework.Function.Function>();
			foreach (var actionDescriptor in actionDescriptorCollectionProvider.ActionDescriptors.Items)
			{
				functions.Add(actionDescriptor.GetFunction());
			}

			return functions;
		}
	}
}