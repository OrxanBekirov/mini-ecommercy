using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;
namespace Entities.Concrete.Auth
{
    public class AppUser:IdentityUser
    {
        public string FullName { get; set; }
        public string? ProfileImageUrl { get; set; }
        public string? ProfileImagePublicId { get; set; } // cloudinary üçün

        public Cart  Cart { get; set; }
        public List<Order> Orders { get; set; } //bir user birden cox sifari ola biler
        public ICollection<Wishlist> Wishlists{ get; set; }
    }
}
