using Entities.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Dtos.Payment
{
    public class PaymentGetDto
    {
        public int Id { get; set; }
        public int OrderId { get; set; }

        public decimal Amount { get; set; }
        public PaymentMethod Method { get; set; }
        public PaymentStatus Status { get; set; }

        public DateTime? PaidAt { get; set; }
        public string ProviderReference { get; set; }
        public string FailureReason { get; set; }
    }
}
