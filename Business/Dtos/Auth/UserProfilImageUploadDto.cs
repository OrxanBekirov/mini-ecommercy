using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Dtos.Auth
{
    public  class UserProfilImageUploadDto
    {
        public IFormFile File { get; set; }
    }
}
