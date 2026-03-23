using Business.Dtos.CartItem;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Dtos.Cart
{
    public class CartGetDto
    {
        public int CartId { get; set; }
        public List<CartItemGetDto> Items { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
