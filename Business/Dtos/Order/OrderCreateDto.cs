using Business.Dtos.OrderItem;
using Entities.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Dtos.Order
{
    public class OrderCreateDto
    {
        public string ShippingAddress { get; set; }
        public string Note { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public List<OrderItemCreateDto> Items { get; set; }
    }
}
