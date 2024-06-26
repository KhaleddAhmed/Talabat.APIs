﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Talabat.APIs.DTOs;
using Talabat.APIs.Errors;
using Talabat.Core.Entities.OrderAggregate;
using Talabat.Core.Services.Contract;

namespace Talabat.APIs.Controllers
{

	[ApiExplorerSettings(IgnoreApi = true)]
	[Authorize]

	public class OrdersController : BaseApiController
	{
		private readonly IOrderService _orderService;
		private readonly IMapper _mapper;

		public OrdersController(IOrderService orderService, IMapper mapper)
		{
			_orderService = orderService;
			_mapper = mapper;
		}
		[ProducesResponseType(typeof(OrderToReturnDto), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
		[HttpPost]
		public async Task<ActionResult<OrderToReturnDto>> CreateOrder(OrderDto orderDto)
		{
			var buyerEmail = User.FindFirstValue(ClaimTypes.Email); //new 
			var address = _mapper.Map<AddressDto, Address>(orderDto.ShippingAddress);
			var order = await _orderService.CreateOrder(buyerEmail, orderDto.BasketId, orderDto.DeliveryMethodId, address);//new
																														   //var order = await _orderService.CreateOrder(orderDto.BuyerEmail, orderDto.BasketId, orderDto.DeliveryMethodId, address);//old

			if (order is null)
			{
				return BadRequest(new ApiResponse(400));
			}
			return Ok(_mapper.Map<Order, OrderToReturnDto>(order));
		}
		[HttpGet]
		//public async Task<ActionResult<IReadOnlyList<OrderToReturnDto>>> GetOrdersForUser(string email)
		//{
		//	var orders = await _orderService.GetOrdersForUserAsync(email);
		//	return Ok(_mapper.Map<IReadOnlyList<Order>, IReadOnlyList<OrderToReturnDto>>(orders));
		//}
		public async Task<ActionResult<IReadOnlyList<OrderToReturnDto>>> GetOrdersForUser()
		{
			var buyerEmail = User.FindFirstValue(ClaimTypes.Email);
			var orders = await _orderService.GetOrdersForUserAsync(buyerEmail);
			return Ok(_mapper.Map<IReadOnlyList<Order>, IReadOnlyList<OrderToReturnDto>>(orders));
		}

		[HttpGet("{id}")]
		[ProducesResponseType(typeof(OrderToReturnDto), StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
		//public async Task<ActionResult<OrderToReturnDto>> GetOrderForUser(string email ,int id)
		//{
		//	var order = await _orderService.GetOrderByIdForUserAsync( email , id);
		//	if (order is null)
		//		return NotFound(new ApiResponse(404));
		//	return Ok(_mapper.Map<Order,OrderToReturnDto>(order));
		//}
		public async Task<ActionResult<OrderToReturnDto>> GetOrderForUser(int id)
		{
			var buyerEmail = User.FindFirstValue(ClaimTypes.Email);
			var order = await _orderService.GetOrderByIdForUserAsync(buyerEmail, id);
			if (order is null) return NotFound(new ApiResponse(404));
			return Ok(_mapper.Map<OrderToReturnDto>(order));
		}

		[HttpGet("deliveryMethods")]
		public async Task<ActionResult<IReadOnlyList<DeliveryMethod>>> GetDeliveryMethod()
		{
			var deliveryMethods = await _orderService.GetDeliveryMethodsAsync();
			return Ok(deliveryMethods);
		}
	}
}
