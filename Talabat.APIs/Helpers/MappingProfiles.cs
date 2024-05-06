using AutoMapper;
using Talabat.APIs.DTOs;
using Talabat.Core.Entities;
using Talabat.Core.Entities.Identity;

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
			CreateMap<Address,AddressDto>().ReverseMap();
		}

	}
}
