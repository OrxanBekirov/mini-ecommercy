using Core.Entities.BaseEntity;
using Entities.Concrete.Auth;
using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.Concrete
{
    public class Wishlist:BaseEntity
    {
        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}
