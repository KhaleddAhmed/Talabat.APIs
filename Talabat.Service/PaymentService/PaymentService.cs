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
using Product = Talabat.Core.Entities.Product;

namespace Talabat.Service.PaymentService
{
	public class PaymentService : IPaymentService
	{
		private readonly IConfiguration _configuration;
		private readonly IBasketRepository _basketRepo;
		private readonly IUnitOfWork _unitOfWork;

		public PaymentService(IConfiguration configuration,IBasketRepository basketRepo,IUnitOfWork unitOfWork)
        {
			_configuration = configuration;
			_basketRepo = basketRepo;
			_unitOfWork = unitOfWork;
		}
        public async Task<CustomerBasket?> CreateOrUpdatePaymentIntent(string basketId)
		{
			StripeConfiguration.ApiKey = _configuration["StripeSettings:SecretKey"];
			var basket=await _basketRepo.GetBasketAsync(basketId);

			if (basket is null)
				return null;

			var shippingPrice = 0m;

			if(basket.DeliveryMethodId.HasValue)
			{
				var deliveryMethod=await _unitOfWork.Repository<DeliveryMethod>().GetAsync(basket.DeliveryMethodId.Value);
				 shippingPrice = deliveryMethod.Cost;
				basket.ShippingPrice = shippingPrice;
			}
			if(basket.Items?.Count>0)
			{
				var productRepo = _unitOfWork.Repository<Product>();
				foreach (var item in basket.Items) 
				{
					var product = await productRepo.GetAsync(item.Id);
					if(item.Price!=product.Price)
						item.Price = product.Price;

				}
			}

			PaymentIntent paymentIntent;
			PaymentIntentService paymentIntentService = new PaymentIntentService();

			if(string.IsNullOrEmpty( basket.PaymentIntentID))//creae new payment intent
			{
				var options=new PaymentIntentCreateOptions()
				{
					Amount=(long)basket.Items.Sum(item=>item.Price*item.Quantity)+(long)shippingPrice*100,
					Currency="usd",
					PaymentMethodTypes=new List<string>() { "card"}
				};
			  paymentIntent= await paymentIntentService.CreateAsync(options); //Integration with stripe

				basket.PaymentIntentID = paymentIntent.Id;
				basket.ClientSecret = paymentIntent.ClientSecret;

			}
			else //update existing payment intent
			{
				var options = new PaymentIntentUpdateOptions()
				{
					Amount = (long)basket.Items.Sum(item => item.Price * item.Quantity) + (long)shippingPrice * 100,

				};

				await paymentIntentService.UpdateAsync(basket.PaymentIntentID,options);

			}

			await _basketRepo.UpdateBasketAsync(basket);
			return basket;

		}
	}
}
