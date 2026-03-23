using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Dtos.Auth
{
    public class AuthResponseDto
    {
        public string Token { get; set; }
       
        public string Email { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string? ProfileImageUrl { get; set; }
        public DateTime Expires { get; set; }
    }
}
