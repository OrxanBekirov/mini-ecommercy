using Core.Entities.BaseEntity;
using Entities.Concrete.Auth;
using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.Concrete
{
    public class Cart:BaseEntity
    {
        public string AppUserId { get; set; }
        public AppUser AppUser { get; set; }
        public List<CartItem> CartItems { get; set; }
    }
}
