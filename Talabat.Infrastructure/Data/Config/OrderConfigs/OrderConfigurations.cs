﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.OrderAggregate;

namespace Talabat.Infrastructure.Data.Config.OrderConfigs
{
	public class OrderConfigurations : IEntityTypeConfiguration<Order>
	{
		public void Configure(EntityTypeBuilder<Order> builder)
		{
			builder.OwnsOne(O => O.ShippingAddress, ShippingAddress => ShippingAddress.WithOwner());

			builder.Property(O => O.Status)
				.HasConversion(
				(OStatus)=>OStatus.ToString(),
				(OStatus)=>(OrderStatus)Enum.Parse(typeof(OrderStatus), OStatus)

				);
			builder.Property(O => O.SubTotal)
				.HasColumnType("decimal(12,2)");


			builder.HasOne(O => O.DeliveryMethod)
				.WithMany()
				.OnDelete(DeleteBehavior.SetNull);

			builder.HasMany(O => O.OrderItems)
			.WithOne()
			.OnDelete(DeleteBehavior.Cascade);
				
		}
	}
}
