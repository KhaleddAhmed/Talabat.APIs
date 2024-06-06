using System.ComponentModel.DataAnnotations;
using Talabat.Core.Entities.OrderAggregate;

namespace Talabat.APIs.DTOs
{
	public class OrderDto
	{
  
        [Required]
        public string BasketId { get; set; }
        [Required]
        public int DeliveryMethodId { get; set; }
        public AddressDto  ShippingAddress { get; set; }
    }
}
