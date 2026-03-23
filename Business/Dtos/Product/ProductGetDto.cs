using Entities.Concrete;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Dtos.Product
{
    public class ProductGetDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }

        public int BrandId { get; set; }
        public string BrandName { get; set; }

        public int CategoryId { get; set; }
        public string CategoryName { get; set; }

        public List<string> ImageUrls { get; set; } = new();
    }
}
