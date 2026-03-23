using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Dtos.OrderItem
{
    public class OrderItemGetDto
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal LineTotal { get; set; }
    }
}
