using Microsoft.Extensions.Configuration;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core;
using Talabat.Core.Entities;
using Talabat.Core.Entities.OrderAggregate;
using Talabat.Core.Repositories.Contract;
using Talabat.Core.Services.Contract;
using Talabat.Core.Specifications.Order_Specs;
using Product = Talabat.Core.Entities.Product;

namespace Talabat.Service.PaymentService
{
	public class PaymentService : IPaymentService
	{
		private readonly IConfiguration _configuration;
		private readonly IBasketRepository _basketRepository;
		private readonly IUnitOfWork _unitOfWork;

		public PaymentService(IConfiguration configuration, IBasketRepository basketRepository, IUnitOfWork unitOfWork)
		{
			_configuration = configuration;
			_basketRepository = basketRepository;
			_unitOfWork = unitOfWork;
		}
		public async Task<CustomerBasket?> CreateOrUpdatePaymentIntent(string basketId)
		{
			StripeConfiguration.ApiKey = _configuration["StripeSettings:SecretKey"];
			var basket = await _basketRepository.GetBasketAsync(basketId);
			if (basket is null)
			{
				return null;
			}

			var shippingPrice = 0m;

			if (basket.DeliveryMethodId.HasValue)
			{
				var deliveryMethod = await _unitOfWork.Repository<DeliveryMethod>().GetAsync(basket.DeliveryMethodId.Value);
				shippingPrice = deliveryMethod.Cost;
				basket.ShippingPrice = shippingPrice;
			}
			if (basket.Items?.Count > 0)
			{
				var productRepo = _unitOfWork.Repository<Product>();
				foreach (var item in basket.Items)
				{
					var product = await productRepo.GetAsync(item.Id);
					if (item.Price != product.Price)
					{
						item.Price = product.Price;
					}
				}
			}
			PaymentIntent paymentIntent;
			PaymentIntentService paymentIntentService = new();
			if (string.IsNullOrEmpty(basket.PaymentIntentID)) // create new paymentIntent
			{
				var options = new PaymentIntentCreateOptions()
				{
					Amount = (long)basket.Items.Sum(Item => Item.Price * 100 * Item.Quantity) + (long)shippingPrice,
					Currency = "usd",
					PaymentMethodTypes = new List<string>() { "card" }
				};
				paymentIntent = await paymentIntentService.CreateAsync(options); //Inegration with stripe 
				basket.PaymentIntentID = paymentIntent.Id;
				basket.ClientSecret = paymentIntent.ClientSecret;
			}
			else // update existing paymentIntent
			{
				var options = new PaymentIntentUpdateOptions()
				{
					Amount = (long)basket.Items.Sum(Item => Item.Price * 100 * Item.Quantity) + (long)shippingPrice
				};
				await paymentIntentService.UpdateAsync(basket.PaymentIntentID, options);
			}
			await _basketRepository.UpdateBasketAsync(basket);
			return basket;
		}

		public async Task<Core.Entities.OrderAggregate.Order?> UpdateOrderStatus(string paymentIntentId, bool isPaid)
		{
			var orderRepo = _unitOfWork.Repository<Core.Entities.OrderAggregate.Order>();
			var spec = new OrderWithPaymentIntentSpecification(paymentIntentId);
			var order = await orderRepo.GetWithSpec(spec);
			if (order is null)
			{
				return null;
			}
			if (isPaid)
				order.Status = OrderStatus.PaymentRecieved;
			else
				order.Status = OrderStatus.PaymentFailed;
			orderRepo.Updater(order);
			await _unitOfWork.CompleteAsync();
			return order;
		}
	}
}
