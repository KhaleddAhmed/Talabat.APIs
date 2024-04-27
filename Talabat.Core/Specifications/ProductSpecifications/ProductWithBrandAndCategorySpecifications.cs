using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;

namespace Talabat.Core.Specifications.ProductSpecifications
{
	public class ProductWithBrandAndCategorySpecifications:BaseSpecifications<Product>
	{
        public ProductWithBrandAndCategorySpecifications(ProductSpecParams specParams):base(P=>
		
		                (!specParams.BrandId.HasValue||P.BrandId==specParams.BrandId)&&
		                 (!specParams.CategoryId.HasValue||P.CategoryId==specParams.CategoryId)
		
		
		)
		{
			AddIncludes();

			if (!string.IsNullOrEmpty(specParams.Sort))
			{
				switch (specParams.Sort)
				{
					case "priceAsc":
						AddOrderBy(P => P.Price);
						break;

					case "priceDesc":
						AddOrderByDesc(P => P.Price);
						break;

					default:
						AddOrderBy(P => P.Name);
						break;

				}
			}

			else
				AddOrderBy(P => P.Name);

			//total products =18
			//page size =5
			//page index=3 [talet 5]
			ApplyPagination((specParams.PageIndex-1)*specParams.PageSize /*skip 10*/,specParams.PageSize /*take 5*/);
		}



		//This Constructor will be used for creating object, That will be used  in passing critires
		public ProductWithBrandAndCategorySpecifications(int id):base(P=>P.Id==id) 
        {
			AddIncludes();
            
        }

		private void AddIncludes()
		{
			Includes.Add(P => P.Brand);
			Includes.Add(P => P.Cateogry);
		}


	}
}
