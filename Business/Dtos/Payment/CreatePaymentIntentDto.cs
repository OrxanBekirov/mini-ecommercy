using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Dtos.Payment
{
    public class CreatePaymentIntentDto
    {
        public int OrderId { get; set; }
    }
}
