using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Dtos.Wishlist
{
    public class WishlistGetDto
    {
        public int WishlistId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int BrandId { get; set; }
        public string BrandName { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public List<string> ImageUrls { get; set; }

    }
}
