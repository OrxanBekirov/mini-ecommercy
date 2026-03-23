using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Dtos.Product
{
    public class ProductCreateDto
    {

        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }

        public int BrandId { get; set; }
        public int CategoryId { get; set; }
    }
}
