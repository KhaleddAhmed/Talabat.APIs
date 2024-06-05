﻿using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.DTOs;
using Talabat.APIs.Errors;
using Talabat.Core.Entities.OrderAggregate;
using Talabat.Core.Services.Contract;

namespace Talabat.APIs.Controllers
{

	public class OrdersController : BaseApiController
	{
		private readonly IOrderService _orderService;
		private readonly IMapper _mapper;

		public OrdersController(IOrderService orderService,IMapper mapper)
        {
			_orderService = orderService;
			_mapper = mapper;
		}

		[ProducesResponseType(typeof(OrderToReturnDto),StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(ApiResponse),StatusCodes.Status400BadRequest)]
		[HttpPost]

		public async Task<ActionResult<OrderToReturnDto>> CreateOrder(OrderDto orderDto)
		{
			var address = _mapper.Map<AddressDto,Address>(orderDto.ShippingAddress);
			var order=await _orderService.CreateOrderAsync(orderDto.BuyerEmail, orderDto.DeliveryMethodId, orderDto.BasketId, address);

			if (order is null)
				return BadRequest(new ApiResponse(400));

			return Ok(_mapper.Map<Order,OrderToReturnDto>(order));
		}

		[HttpGet]
		public async Task<ActionResult<IReadOnlyList<OrderToReturnDto>>> GetOrdersForUser(string email)
		{
			var orders=await _orderService.GetOrdersFouUserAsync(email);
			return Ok(_mapper.Map<IReadOnlyList<Order>,IReadOnlyList<OrderToReturnDto>>(orders));
		}
		[ProducesResponseType(typeof(OrderToReturnDto),StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(ApiResponse),StatusCodes.Status404NotFound)]
		[HttpGet("{id}")]
		public async Task<ActionResult<Order>> GetOrdersForUser(int id,string email)
		{
			var order = await _orderService.GetOrderByIdForUserAsync(email, id);
			if (order is null)
				return NotFound(new ApiResponse(404));

			return Ok(_mapper.Map< OrderToReturnDto>(order));

		}
	}
}