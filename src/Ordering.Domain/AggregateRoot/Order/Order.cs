using System;
using System.Collections.Generic;
using System.Linq;
using MSFramework.Domain;
using Ordering.Domain.Events;

namespace Ordering.Domain.AggregateRoot.Order
{
	public class Order : AggregateRootBase<Guid>
	{
		// DDD Patterns comment
		// Using private fields, allowed since EF Core 1.1, is a much better encapsulation
		// aligned with DDD Aggregates and Domain Entities (Instead of properties and property collections)
		private DateTime _orderDate;
		private int _orderStatusId;

		private string _description;

		// Draft orders have this set to true. Currently we don't check anywhere the draft status of an Order, but we could do it if needed
		private bool _isDraft;

		// DDD Patterns comment
		// Using a private collection field, better for DDD Aggregate's encapsulation
		// so OrderItems cannot be added from "outside the AggregateRoot" directly to the collection,
		// but only through the method OrderAggrergateRoot.AddOrderItem() which includes behaviour.
		private readonly List<OrderItem> _orderItems;
		private int? _paymentMethodId;

		public OrderStatus OrderStatus => OrderStatus.From(_orderStatusId);

		public Guid? BuyerId { get; private set; }

		public IReadOnlyCollection<OrderItem> OrderItems => _orderItems;

		// Address is a Value Object pattern example persisted as EF Core 2.0 owned entity
		public Address Address { get; private set; }

		public static Order NewDraft()
		{
			var order = new Order {_isDraft = true};
			return order;
		}

		protected Order()
		{
			_orderItems = new List<OrderItem>();
			_isDraft = false;
		}

		public Order(string userId, string userName, Address address, int cardTypeId, string cardNumber,
			string cardSecurityNumber,
			string cardHolderName, DateTime cardExpiration, Guid? buyerId = null, int? paymentMethodId = null) : this()
		{
			BuyerId = buyerId;
			_paymentMethodId = paymentMethodId;
			_orderStatusId = OrderStatus.Submitted.Id;

			_orderDate = DateTime.UtcNow;
			Address = address;

			// Add the OrderStarterDomainEvent to the domain events collection 
			// to be raised/dispatched when comitting changes into the Database [ After DbContext.SaveChanges() ]
			AddOrderStartedDomainEvent(userId, userName, cardTypeId, cardNumber,
				cardSecurityNumber, cardHolderName, cardExpiration);
		}

		// DDD Patterns comment
		// This Order AggregateRoot's method "AddOrderitem()" should be the only way to add Items to the Order,
		// so any behavior (discounts, etc.) and validations are controlled by the AggregateRoot 
		// in order to maintain consistency between the whole Aggregate. 
		public void AddOrderItem(int productId, string productName, decimal unitPrice, decimal discount,
			string pictureUrl, int units = 1)
		{
			var existingOrderForProduct = _orderItems
				.SingleOrDefault(o => o.ProductId == productId);

			if (existingOrderForProduct != null)
			{
				//if previous line exist modify it with higher discount  and units..

				if (discount > existingOrderForProduct.Discount)
				{
					existingOrderForProduct.SetDiscount(discount);
				}

				existingOrderForProduct.AddUnits(units);
			}
			else
			{
				//add validated new order item

				var orderItem = new OrderItem(productId, productName, unitPrice, discount, pictureUrl, units);
				_orderItems.Add(orderItem);
			}
		}

		public void SetPaymentId(int id)
		{
			_paymentMethodId = id;
		}

		public void SetBuyerId(Guid id)
		{
			BuyerId = id;
		}

		public void SetAwaitingValidationStatus()
		{
			if (_orderStatusId == OrderStatus.Submitted.Id)
			{
				AddEvent(new OrderStatusChangedToAwaitingValidationDomainEvent(_orderItems));
				_orderStatusId = OrderStatus.AwaitingValidation.Id;
			}
		}

		public void SetStockConfirmedStatus()
		{
			if (_orderStatusId == OrderStatus.AwaitingValidation.Id)
			{
				AddEvent(new OrderStatusChangedToStockConfirmedDomainEvent());

				_orderStatusId = OrderStatus.StockConfirmed.Id;
				_description = "All the items were confirmed with available stock.";
			}
		}

		public void SetPaidStatus()
		{
			if (_orderStatusId == OrderStatus.StockConfirmed.Id)
			{
				AddEvent(new OrderStatusChangedToPaidDomainEvent(OrderItems));

				_orderStatusId = OrderStatus.Paid.Id;
				_description =
					"The payment was performed at a simulated \"American Bank checking bank account ending on XX35071\"";
			}
		}

		public void SetShippedStatus()
		{
			if (_orderStatusId != OrderStatus.Paid.Id)
			{
				StatusChangeException(OrderStatus.Shipped);
			}

			_orderStatusId = OrderStatus.Shipped.Id;
			_description = "The order was shipped.";
			AddEvent(new OrderShippedDomainEvent(this));
		}

		public void SetCancelledStatus()
		{
			if (_orderStatusId == OrderStatus.Paid.Id ||
			    _orderStatusId == OrderStatus.Shipped.Id)
			{
				StatusChangeException(OrderStatus.Cancelled);
			}

			_orderStatusId = OrderStatus.Cancelled.Id;
			_description = $"The order was cancelled.";
			AddEvent(new OrderCancelledDomainEvent(this));
		}

		public void SetCancelledStatusWhenStockIsRejected(IEnumerable<int> orderStockRejectedItems)
		{
			if (_orderStatusId == OrderStatus.AwaitingValidation.Id)
			{
				_orderStatusId = OrderStatus.Cancelled.Id;

				var itemsStockRejectedProductNames = OrderItems
					.Where(c => orderStockRejectedItems.Contains(c.ProductId))
					.Select(c => c.ProductName);

				var itemsStockRejectedDescription = string.Join(", ", itemsStockRejectedProductNames);
				_description = $"The product items don't have stock: ({itemsStockRejectedDescription}).";
			}
		}

		private void AddOrderStartedDomainEvent(string userId, string userName, int cardTypeId, string cardNumber,
			string cardSecurityNumber, string cardHolderName, DateTime cardExpiration)
		{
			var orderStartedDomainEvent = new OrderStartedDomainEvent(this, userId, userName, cardTypeId,
				cardNumber, cardSecurityNumber,
				cardHolderName, cardExpiration);

			AddEvent(orderStartedDomainEvent);
		}

		private void StatusChangeException(OrderStatus orderStatusToChange)
		{
			throw new OrderingException(
				$"Is not possible to change the order status from {OrderStatus.Name} to {orderStatusToChange.Name}.");
		}

		public decimal Total => _orderItems.Sum(o => o.Units * o.UnitPrice);
	}
}