using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Dtos.Product
{
    
        public class ProductImageCreateDto
        {
        public int ProductId { get; set; }
        public string Url { get; set; }
        public string PublicId { get; set; }
        public bool IsMain { get; set; }
    }
    
}
