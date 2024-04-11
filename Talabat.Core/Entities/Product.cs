using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Talabat.Core.Entities
{
	public class Product:BaseEntity
	{
        public string Name { get; set; }
		public string Description { get; set; }
        public string PictureUrl { get; set; }

        public decimal Price { get; set; }

        public int BrandId { get; set; } //Fk Column=>ProductBrand

        public ProductBrand Brand { get; set; } //Navigational property[One]

        public int CategoryId { get; set; }//Fk Column=>ProductCateogry


		public ProductCategory Cateogry { get; set; } //Navigational property[One]

	}
}
