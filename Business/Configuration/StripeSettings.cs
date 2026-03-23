using System;
using System.Collections.Generic;
using System.Text;

namespace Business.Configuration
{
    public class StripeSettings
    {
        public string SecretKey { get; set; }
        public string WebhookSecret { get; set; }
        public string Currency { get; set; } = "usd";
    }
}
