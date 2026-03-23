using Entities.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Dtos.Order
{
    public class OrderFromCartDto
    {
        public string ShippingAddress { get; set; }
        public string Note { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
    }
}
