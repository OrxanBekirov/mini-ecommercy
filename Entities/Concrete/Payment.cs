using Core.Entities.BaseEntity;
using Entities.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.Concrete
{
    public class Payment:BaseEntity
    {

        public int OrderId { get; set; }
        public Order Order { get; set; }

        public decimal Amount { get; set; }

        public PaymentMethod Method { get; set; } // Card, Cash, Stripe

        public PaymentStatus Status { get; set; }
        public string Currency { get; set; } = "azn";
        public DateTime PaymentDate { get; set; } = DateTime.Now;
        public DateTime? PaidAt { get; set; }
        public string Provider { get; set; } = "Stripe";
        public string? FailureReason { get; set; }
        public string ProviderReference { get; set; }
    }
}
