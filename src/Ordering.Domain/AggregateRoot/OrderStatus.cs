namespace Ordering.Domain.AggregateRoot
{
	public enum OrderStatus
	{
		Submitted,
		AwaitingValidation,
		StockConfirmed,
		Paid,
		Shipped,
		Cancelled
	}
}