using Microsoft.AspNetCore.Mvc.Filters;

namespace MSFramework.AspNetCore.Filters
{
	public static class ServiceCollectionExtensions
	{
		public static FilterCollection UseUnitOfWork(this FilterCollection filters)
		{
			filters.Add<UnitOfWork>(FilterOrders.UnitOfWork);
			return filters;
		}

		public static FilterCollection UseFunctionFilter(this FilterCollection filters)
		{
			filters.Add<FunctionFilter>(FilterOrders.FunctionFilter);
			return filters;
		}

		public static FilterCollection UseAudit(this FilterCollection filters)

		{
			filters.Add<Audit>(FilterOrders.Audit);
			return filters;
		}

		public static FilterCollection UseGlobalExceptionFilter(this FilterCollection filters)

		{
			filters.Add<GlobalExceptionFilter>();
			return filters;
		}

		public static FilterCollection UseInvalidModelStateFilter(this FilterCollection filters)
		{
			filters.Add<InvalidModelStateFilter>(FilterOrders.InvalidModelStateFilter);
			return filters;
		}
	}
}