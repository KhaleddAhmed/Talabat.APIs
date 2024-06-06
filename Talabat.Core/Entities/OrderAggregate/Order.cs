﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Talabat.Core.Entities.OrderAggregate
{
	public class Order:BaseEntity
	{
        private Order()
        {
            
        }
        public Order(string buyerEmail,Address shippingAddress,int? deliveryMethodId,ICollection<OrderItem> orderItems,decimal subTotal,string paymentIntentId )
        {
            BuyerEmail = buyerEmail;
            ShippingAddress = shippingAddress;
            DeliveryMethodId = deliveryMethodId;
            OrderItems = orderItems;
            SubTotal = subTotal;
            PaytmentIntent = paymentIntentId;
        }
        public string BuyerEmail { get; set; }

        public DateTimeOffset OrderDate { get; set; } = DateTimeOffset.UtcNow;

        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        public Address ShippingAddress { get; set; } = null!;

        public int? DeliveryMethodId { get; set; }//Foreign Key

        public DeliveryMethod? DeliveryMethod { get; set; } //Navigational property One
        
        public ICollection<OrderItem> OrderItems { get; set; }=new HashSet<OrderItem>();

        public decimal SubTotal { get; set; }
        //[NotMapped]
        //public decimal Total { get { return DeliveryMethod.Cost + SubTotal; } }

        public decimal GetTotal()=>SubTotal+DeliveryMethod.Cost;

        public string PaytmentIntent { get; set; } = string.Empty;

    }
}
