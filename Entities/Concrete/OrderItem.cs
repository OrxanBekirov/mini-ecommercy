using Core.Entities.BaseEntity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.Concrete
{
    public class OrderItem:BaseEntity
    {

        public int OrderId { get; set; }
        public Order Order { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }

        public string ProductNameSnapshot { get; set; } // vacibdir
        public decimal UnitPrice { get; set; }

        public int Quantity { get; set; }
        public decimal LineTotal => UnitPrice * Quantity;

    }
}
