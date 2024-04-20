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
        public ProductWithBrandAndCategorySpecifications():base()
		{
			AddIncludes();
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
