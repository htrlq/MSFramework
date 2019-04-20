using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Logging;
using MSFramework.EntityFrameworkCore;
using MSFramework.EventBus;

namespace MSFramework.EventSource.EntityFrameworkCore
{
	public class EventSourceDbContext : DbContextBase
	{
		public EventSourceDbContext(DbContextOptions options, IEntityConfigurationTypeFinder typeFinder,
			IEventBus mediator,
			ILoggerFactory loggerFactory) : base(options, typeFinder, mediator, loggerFactory)
		{
		}
	}
}