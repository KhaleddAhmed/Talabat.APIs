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

namespace Talabat.Service.OrderService
{
	public class OrderService : IOrderService
	{
		private readonly IBasketRepository _basketRepository;
		private readonly IUnitOfWork _unitOfWork;
		///private readonly IGenericRepository<Product> _productRepo;
		///private readonly IGenericRepository<DeliveryMethod> _deliveryRepo;
		///private readonly IGenericRepository<Order> _orderRepo;

		public OrderService(IBasketRepository  basketRepository,/*IGenericRepository<Product> productRepo,IGenericRepository<DeliveryMethod> deliveryRepo,IGenericRepository<Order> orderRepo*/IUnitOfWork unitOfWork)
        {
			_basketRepository = basketRepository;
			_unitOfWork = unitOfWork;
			///_productRepo = productRepo;
			///_deliveryRepo = deliveryRepo;
			///_orderRepo = orderRepo;
		}
        public async Task<Order?> CreateOrderAsync(string buyerEmail, int deliveryMethodId, string basketId, Address shippingAddress)
		{
			//1.Get Basket from Basket Repo
			var basket = await _basketRepository.GetBasketAsync(basketId);

			//2.Get selected Items at basket from Product Repo
			var orderItems=new List<OrderItem>();
			if(basket?.Items?.Count > 0) 
			{
				foreach(var item in basket.Items)
				{
					var product = await _unitOfWork.Repository<Product>().GetAsync(item.Id);
					var productItemOrdered = new ProductItemOrdered(product.Id, product.Name, product.PictureUrl);
					var orderItem = new OrderItem(productItemOrdered, product.Price, item.Quantity);
					orderItems.Add(orderItem);
				}
			}

			//3.calculate subTotal
			var subTotal=orderItems.Sum(I=> I.Price * I.Quantity);

			//4.get deliveryMethod
			var deliveryMethod = await _unitOfWork.Repository<DeliveryMethod>().GetAsync(deliveryMethodId);

			//5.Create Order

			var order=new Order(buyerEmail,shippingAddress,deliveryMethodId,orderItems,subTotal);

			
			 _unitOfWork.Repository<Order>().Add(order);
			//6-save to database [TODO]
			var result=await  _unitOfWork.CompleteAsync() ;
			if (result <= 0)
				return null;

			return order;

		}

		public Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodsForUserAsync()
		{
			throw new NotImplementedException();
		}

		public async Task<Order?> GetOrderByIdForUserAsync(string buyerEmail, int orderId)
		{
			var orderRepo=_unitOfWork.Repository<Order>();
			var orderSpecification = new OrderSpecifications(orderId, buyerEmail);
			var order = await orderRepo.GetWithSpec(orderSpecification);

			
			return order;
		}

		public async Task<IReadOnlyList<Order>> GetOrdersFouUserAsync(string buyerEmail)
		{
			var orderRepo=_unitOfWork.Repository<Order>();
			var spec = new OrderSpecifications(buyerEmail);
			var orders=await orderRepo.GetAllWithSpec(spec);

			return orders;

		}
	}
}
