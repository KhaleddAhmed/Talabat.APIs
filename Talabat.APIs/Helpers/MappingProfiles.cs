using AutoMapper;
using Talabat.APIs.DTOs;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Identity;
using Talabat.Core.Entities.OrderAggregate;

namespace Talabat.APIs.Helpers
{
	public class MappingProfiles:Profile
	{
	

		public MappingProfiles()
        {
			CreateMap<Product, ProductToReturnDto>()
				.ForMember(d => d.Brand, O => O.MapFrom(s => s.Brand.Name))
				.ForMember(d => d.Cateogry, O => O.MapFrom(s => s.Cateogry.Name))
				//.ForMember(P => P.PictureUrl, O => O.MapFrom(S => $"{_configuration["ApiBaseUrl"]}/{S.PictureUrl}"));
				.ForMember(P => P.PictureUrl, O => O.MapFrom<ProductPictureUrlResolver>());

			CreateMap<CustomerBasketDto, CustomerBasket>();
			CreateMap<BasketItemDto, BasketItem>();
			CreateMap<Core.Entities.Identity.Address, AddressDto>().ReverseMap();
			CreateMap<AddressDto, Core.Entities.OrderAggregate.Address>();

			CreateMap<Order, OrderToReturnDto>()
				.ForMember(d => d.DeliveryMethod, O => O.MapFrom(S => S.DeliveryMethod.ShortName))
				 .ForMember(d=>d.DeliveryMethodCost,O=>O.MapFrom(S=>S.DeliveryMethod.Cost));

			CreateMap<OrderItem, OrderItemDto>()
				.ForMember(d => d.ProductName, O => O.MapFrom(S => S.Product.ProductName))
				.ForMember(d => d.ProductId, O => O.MapFrom(S => S.Product.ProductId))
				.ForMember(d => d.PictureUrl, O => O.MapFrom(S => S.Product.PictureUrl))
				.ForMember(d=>d.PictureUrl,O=>O.MapFrom<OrderItemPictureUrlResolver>());
		}

	}
}
