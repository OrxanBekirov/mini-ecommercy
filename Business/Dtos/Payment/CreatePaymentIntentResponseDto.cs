using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Dtos.Payment
{
    public class CreatePaymentIntentResponseDto
    {
        public string ClientSecret { get; set; }
        public string PaymentIntentId { get; set; }
    }
}
