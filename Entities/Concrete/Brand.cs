using Core.Entities.BaseEntity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.Concrete
{
    public class Brand:BaseEntity
    {
        public string Name { get; set; }
        public List<Product> Products { get; set; }
    }
}
