using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MSFramework.EntityFrameworkCore;
using MSFramework.EventBus;

namespace Ordering.Infrastructure
{
	public class OrderingContext : DbContextBase
	{
		public const string DefaultSchema = "ordering";

		public OrderingContext(DbContextOptions options, IEntityConfigurationTypeFinder typeFinder, IEventBus mediator,
			ILoggerFactory loggerFactory) : base(options, typeFinder, mediator, loggerFactory)
		{
		}
	}
}