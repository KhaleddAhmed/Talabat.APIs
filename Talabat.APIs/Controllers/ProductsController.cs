using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.DTOs;
using Talabat.Core.Entities;
using Talabat.Core.Repositories.Contract;
using Talabat.Core.Specifications;
using Talabat.Core.Specifications.ProductSpecifications;

namespace Talabat.APIs.Controllers
{

	public class ProductsController : BaseApiController
	{
		private readonly IGenericRepository<Product> _productRepo;
		private readonly IMapper _mapper;

		public ProductsController(IGenericRepository<Product> productRepo,IMapper mapper)
        {
			_productRepo = productRepo;
			_mapper = mapper;
		}

		// /api/Products
		[HttpGet]
		public async Task<ActionResult<IEnumerable<ProductToReturnDto>>> GetProducts()
		{
			var spec = new ProductWithBrandAndCategorySpecifications();
			var products = await _productRepo.GetAllWithSpec(spec);

			return Ok(_mapper.Map<IEnumerable<Product>,IEnumerable<ProductToReturnDto>>(products));
		}

		// /api/Products/1
		[HttpGet("{id}")]
		public async Task<ActionResult<ProductToReturnDto>> GetProduct(int id)
		{
			var spec=new ProductWithBrandAndCategorySpecifications(id);
			var product = await _productRepo.GetWithSpec(spec);
			if(product is null)
			   return NotFound(new {Message="Not Found", StatusCode=404});//404

			return Ok(_mapper.Map<Product,ProductToReturnDto>(product));//200

			
		}

    }
}
