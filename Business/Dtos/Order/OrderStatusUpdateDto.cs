using Entities.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Dtos.Order
{
    public class OrderStatusUpdateDto
    {
        public OrderStatus Status { get; set; }
    }
}
