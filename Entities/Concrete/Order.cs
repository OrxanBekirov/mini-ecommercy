using Core.Entities.BaseEntity;
using Entities.Concrete.Auth;
using Entities.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.Concrete
{
    public class Order:BaseEntity
    {

        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }
        public string OrderNumber { get; set; }
        public decimal TotalAmount { get; set; }
        public OrderStatus OrderStatus { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreateAt { get; set; }
        public string ShippingAddress { get; set; }
        public string Note { get; set; }
        public List<OrderItem> OrderItems { get; set; } = new();
        public Payment Payment { get; set; }
    }
}
