using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Dtos.Payment
{
    public class PaymentMarkFailedDto
    {
        public int OrderId { get; set; }
        public string FailureReason { get; set; }
        public string ProviderReference { get; set; }
    }

}
