using System.Threading;
using System.Threading.Tasks;
using MediatR;
using MSFramework.Domain;
using Ordering.Domain.Repository;

namespace Ordering.Application.Command
{
	public class ChangeOrderAddressCommandHandler : IRequestHandler<ChangeOrderAddressCommand>
	{
		private readonly IOrderingRepository _orderRepository;
		private readonly IUnitOfWorkManager _unitOfWorkManager;
		
		public ChangeOrderAddressCommandHandler(IOrderingRepository orderRepository, IUnitOfWorkManager unitOfWorkManager)
		{
			_orderRepository = orderRepository;
			_unitOfWorkManager = unitOfWorkManager;
		}

		/// <summary>
		/// Handler which processes the command when
		/// customer executes cancel order from app
		/// </summary>
		/// <param name="command"></param>
		/// <returns></returns>
		public async Task<Unit> Handle(ChangeOrderAddressCommand command, CancellationToken cancellationToken)
		{
			var order = await _orderRepository.GetAsync(command.OrderId);
			if (order == null)
			{
				return Unit.Value;
			}

			order.ChangeAddress(command.NewAddress);
			await _unitOfWorkManager.CommitAsync();
			return Unit.Value;
		}
	}
}