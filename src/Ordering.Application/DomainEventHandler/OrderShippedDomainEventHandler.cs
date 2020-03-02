using System.Threading.Tasks;
using EventBus;
using MSFramework.Ef;
using Ordering.Domain.AggregateRoot.Event;

namespace Ordering.Application.DomainEventHandler
{
	public class OrderShippedDomainEventHandler : IEventHandler<OrderShippedDomainEvent>
	{
		private readonly DbContextFactory _dbContextFactory;

		public OrderShippedDomainEventHandler(DbContextFactory dbContextFactory)
		{
			_dbContextFactory = dbContextFactory;
		}

		public Task HandleAsync(OrderShippedDomainEvent @event)
		{
			// todo
			return Task.CompletedTask;
		}
	}
}