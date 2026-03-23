using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Dtos.OrderItem
{
    public class OrderItemCreateDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
