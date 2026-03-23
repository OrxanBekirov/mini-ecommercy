using Entities.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Dtos.Media
{
    public class MediaUploadResultDto
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string PublicId { get; set; }

        public MediaOwnerType OwnerType { get; set; }
        public int? OwnerId { get; set; }
        public string? OwnerKey { get; set; }

        public bool IsMain { get; set; }
    }
}
