using Core.Entities.BaseEntity;
using Entities.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.Concrete
{
    public class Media:BaseEntity
    {
        public string Url { get; set; }
        public string PublicId { get; set; }

        public string ResourceType { get; set; } // "image"
        public bool IsMain { get; set; }

        public MediaOwnerType OwnerType { get; set; }
        public bool IsDeleted { get; set; }

        // Product/Brand/Category üçün
        public int? OwnerId { get; set; }

        // User (Identity) üçün string lazım olur
        public string? OwnerKey { get; set; } // AppUserId
    }
}
