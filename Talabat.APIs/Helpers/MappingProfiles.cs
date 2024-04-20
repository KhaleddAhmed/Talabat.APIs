using AutoMapper;
using Talabat.APIs.DTOs;
using Talabat.Core.Entities;

namespace Talabat.APIs.Helpers
{
	public class MappingProfiles:Profile
	{
        public MappingProfiles()
        {
            CreateMap<Product, ProductToReturnDto>()
                .ForMember(d => d.Brand,O=>O.MapFrom(s=>s.Brand.Name))
                .ForMember(d => d.Cateogry, O => O.MapFrom(s=>s.Cateogry.Name));



        }
    }
}
