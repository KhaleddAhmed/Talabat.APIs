using Talabat.Core.Entities.OrderAggregate;

namespace Talabat.APIs.DTOs
{
	public class OrderToReturnDto
	{
		public int Id { get; set; }
		public string BuyerEmail { get; set; }

		public DateTimeOffset OrderDate { get; set; } = DateTimeOffset.UtcNow;

		public string Status { get; set; } 

		public Address ShippingAddress { get; set; } = null!;

		public string DeliveryMethod { get; set; }
        public decimal DeliveryMethodCost { get; set; }

        public ICollection<OrderItemDto> OrderItems { get; set; } = new HashSet<OrderItemDto>();

		public decimal SubTotal { get; set; }

        public decimal Total { get; set; }

        public string PaytmentIntent { get; set; } = string.Empty;
	}
}
