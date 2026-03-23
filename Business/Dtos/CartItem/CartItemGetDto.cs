using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Dtos.CartItem
{
    public class CartItemGetDto
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }

        // BAX BU SƏTİRİ ƏLAVƏ ET:
        public string ImageUrl { get; set; }
    }
}