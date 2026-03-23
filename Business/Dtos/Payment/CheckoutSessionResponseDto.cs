using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Dtos.Payment
{
    public class CheckoutSessionResponseDto
    {
        public string SessionId { get; set; }
        public string CheckoutUrl { get; set; }
    }
}
