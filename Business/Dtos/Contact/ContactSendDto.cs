using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Dtos.Contact
{
    public class ContactSendDto
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Message { get; set; }
        public string Subject { get; set; }

    }
}
