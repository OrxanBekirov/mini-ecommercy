using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Dtos.Cart
{
    public class CartUpdateDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
