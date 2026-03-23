using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Dtos.Payment
{
    public  class PaymentMarkSuccessDto
    {
        public int OrderId { get; set; }
        public string? ProviderReference { get; set; }
    }
}
