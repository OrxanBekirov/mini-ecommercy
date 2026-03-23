using Entities.Enum;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Dtos.Media
{
    public class MediaCreateDto
    {
        public IFormFile File { get; set; }

        public MediaOwnerType OwnerType { get; set; }

        public int? OwnerId { get; set; }     // Product/Brand/Category
        public string? OwnerKey { get; set; }  // UserId

        public bool IsMain { get; set; } = false;
    }
}
