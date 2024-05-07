using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.OrderAggregate;
using Talabat.Core.Services.Contract;

namespace Talabat.Service.OrderService
{
	public class OrderService : IOrderService
	{
		public Task<Order> CreateOrderAsync(string buyerEmail, string deliveryMethodId, string basketId, Address shippingAddress)
		{
			throw new NotImplementedException();
		}

		public Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodsForUserAsync()
		{
			throw new NotImplementedException();
		}

		public Task<Order> GetOrderByIdForUserAsync(string buyerEmail, int orderId)
		{
			throw new NotImplementedException();
		}

		public Task<IReadOnlyList<Order>> GetOrdersFouUserAsync(string buyerEmail)
		{
			throw new NotImplementedException();
		}
	}
}
