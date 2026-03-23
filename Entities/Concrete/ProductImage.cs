using Core.Entities.BaseEntity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.Concrete
{
    public class ProductImage: BaseEntity
    {
        public int ProductId { get; set; }
        public Product Product { get; set; }

        public string ImageUrl { get; set; }
        public string PublicId { get; set; }  // Cloudinary üçün
        public bool IsMain { get; set; }
        public bool IsDeleted { get; set; }
    }
}
