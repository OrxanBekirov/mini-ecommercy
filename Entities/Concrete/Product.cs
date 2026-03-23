using Core.Entities.BaseEntity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.Concrete
{
    public class Product:BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }
        public int BrandId { get; set; }
        public Brand Brand { get; set; }
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        public byte[] RowVersion { get; set; }
        public List<ProductImage> ProductImages { get; set; }
        public bool IsDeleted { get; set; }
        public ICollection<Wishlist> Wishlists { get; set; }
    }
}
