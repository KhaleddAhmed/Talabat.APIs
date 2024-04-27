using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;

namespace Talabat.Core.Specifications.ProductSpecifications
{
	public class ProductsWithFilterationForCount:BaseSpecifications<Product>
	{

		public ProductsWithFilterationForCount(ProductSpecParams specParams):base(P =>

						(!specParams.BrandId.HasValue || P.BrandId == specParams.BrandId) &&
						 (!specParams.CategoryId.HasValue || P.CategoryId == specParams.CategoryId)

		)
		{


		}
	}
}
