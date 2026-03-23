using Business.Dtos.OrderItem;
using Business.Dtos.Payment;
using Entities.Concrete;
using Entities.Concrete.Auth;
using Entities.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Dtos.Order
{
    public class OrderGetDto
    {
        public int Id { get; set; }
        public string AppUserId { get; set; }
        
        public string OrderNumber { get; set; }
        public decimal TotalAmount { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public string ShippingAddress { get; set; }
        public string CheckoutUrl { get; set; }
        public string Note { get; set; }
        public DateTime CreatedAt { get; set; }
        public List<OrderItemGetDto> OrderItems { get; set; } = new();
        public PaymentGetDto Payment { get; set; }
    }
}
