using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.OrderAggregate;

namespace Talabat.Core.Services.Contract
{
	public interface IOrderService
	{
		Task<Order?> CreateOrderAsync(string buyerEmail,int deliveryMethodId,string basketId,Address shippingAddress);

		Task<IReadOnlyList<Order>> GetOrdersFouUserAsync(string buyerEmail);

		Task<Order> GetOrderByIdForUserAsync(string buyerEmail,int orderId);

		Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodsForUserAsync();
	}
}
