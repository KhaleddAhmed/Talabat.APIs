using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities.OrderAggregate;

namespace Talabat.Infrastructure.Data.Config.OrderConfigs
{
	internal class OrderItemConfigurations : IEntityTypeConfiguration<OrderItem>
	{
		public void Configure(EntityTypeBuilder<OrderItem> builder)
		{
			builder.OwnsOne(OI => OI.Product,Product=>Product.WithOwner());


			builder.Property(OI => OI.Price).HasColumnType("decimal(12,2)");



		}
	}
}
