﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;

namespace Talabat.Infrastructure.Data.Config
{
	internal class ProductConfigurations : IEntityTypeConfiguration<Product>
	{
		public void Configure(EntityTypeBuilder<Product> builder)
		{
			builder.Property(P => P.Name).IsRequired().HasMaxLength(100);
			builder.Property(p=>p.Description).IsRequired();
			builder.Property(P=>P.PictureUrl).IsRequired();
			builder.Property(p => p.Price).HasColumnType("decimal(18,2)");


			builder.HasOne(P => P.Brand).WithMany().HasForeignKey(P => P.BrandId);
			builder.HasOne(P => P.Cateogry).WithMany().HasForeignKey(P => P.CategoryId);
		}
	}
}
