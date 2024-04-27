using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.DTOs;
using Talabat.APIs.Errors;
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
		private readonly IGenericRepository<ProductBrand> _brandRepo;
		private readonly IGenericRepository<ProductCategory> _categoryRepo;

		public ProductsController(IGenericRepository<Product> productRepo,IMapper mapper,IGenericRepository<ProductBrand> brandRepo,IGenericRepository<ProductCategory> categoryRepo)
        {
			_productRepo = productRepo;
			_mapper = mapper;
		    _brandRepo = brandRepo;
			_categoryRepo = categoryRepo;
		}

		// /api/Products
		[HttpGet]
		public async Task<ActionResult<IReadOnlyList<ProductToReturnDto>>> GetProducts([FromQuery] ProductSpecParams specParams)
		{
			var spec = new ProductWithBrandAndCategorySpecifications(specParams);
			var products = await _productRepo.GetAllWithSpec(spec);

			return Ok(_mapper.Map<IReadOnlyList<Product>,IReadOnlyList<ProductToReturnDto>>(products));
		}

		// /api/Products/1
		[ProducesResponseType(typeof(ProductToReturnDto),StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(ApiResponse),StatusCodes.Status404NotFound)]
		[HttpGet("{id}")]
		public async Task<ActionResult<ProductToReturnDto>> GetProduct(int id)
		{
			var spec=new ProductWithBrandAndCategorySpecifications(id);
			var product = await _productRepo.GetWithSpec(spec);
			if(product is null)
			   return NotFound(new ApiResponse(404));//404

			return Ok(_mapper.Map<Product,ProductToReturnDto>(product));//200
		}


		[HttpGet("brands")]
		public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetBrands()
		{
			var brands=await _brandRepo.GetAllAsync();
			return Ok(brands);
		}


		[HttpGet("categories")]
		public async Task<ActionResult<IReadOnlyList<ProductCategory>>> GetCategories()
		{
			var categories=await _categoryRepo.GetAllAsync();
			return Ok(categories);
		}

    }
}
