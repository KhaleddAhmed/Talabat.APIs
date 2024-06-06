using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Talabat.APIs.DTOs;
using Talabat.APIs.Errors;
using Talabat.APIs.Helpers;
using Talabat.Core.Entities;
using Talabat.Core.Repositories.Contract;
using Talabat.Core.Services.Contract;
using Talabat.Core.Specifications;
using Talabat.Core.Specifications.ProductSpecifications;

namespace Talabat.APIs.Controllers
{

	public class ProductsController : BaseApiController
	{
		
		private readonly IMapper _mapper;
		private readonly IProductService _productService;
		///private readonly IGenericRepository<Product> _productRepo;
		///private readonly IGenericRepository<ProductBrand> _brandRepo;
		///private readonly IGenericRepository<ProductCategory> _categoryRepo;

		public ProductsController(/*IGenericRepository<Product> productRepo*/IMapper mapper/*,IGenericRepository<ProductBrand> brandRepo,IGenericRepository<ProductCategory> categoryRepo*/,IProductService productService)
        {
			
			_mapper = mapper;
			_productService = productService;
			///_productRepo = productRepo;
			///_brandRepo = brandRepo;
			///_categoryRepo = categoryRepo;
		}

		// /api/Products
		[Authorize]
		[HttpGet]
		public async Task<ActionResult<Pagination <ProductToReturnDto>>> GetProducts([FromQuery] ProductSpecParams specParams)
		{
			
			var products = await _productService.GetProductsAsync(specParams);

			var data=_mapper.Map<IReadOnlyList<Product>,IReadOnlyList<ProductToReturnDto>>(products);
			
			var count=await _productService.GetCountAsync(specParams);

			return Ok(new Pagination<ProductToReturnDto>(specParams.PageIndex,specParams.PageSize,data, count));
		}

		// /api/Products/1
		[ProducesResponseType(typeof(ProductToReturnDto),StatusCodes.Status200OK)]
		[ProducesResponseType(typeof(ApiResponse),StatusCodes.Status404NotFound)]
		[HttpGet("{id}")]
		public async Task<ActionResult<ProductToReturnDto>> GetProduct(int id)
		{
			
			var product =await _productService.GetProductAsync(id);
			if(product is null)
			   return NotFound(new ApiResponse(404));//404

			return Ok(_mapper.Map<Product,ProductToReturnDto>(product));//200
		}


		[HttpGet("brands")]
		public async Task<ActionResult<IReadOnlyList<ProductBrand>>> GetBrands()
		{
			var brands=await _productService.GetBrandsAsync();
			return Ok(brands);
		}


		[HttpGet("categories")]
		public async Task<ActionResult<IReadOnlyList<ProductCategory>>> GetCategories()
		{
			var categories=await _productService.GetCategoriesAsync();
			return Ok(categories);
		}

    }
}
