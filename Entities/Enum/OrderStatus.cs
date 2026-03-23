using System;
using System.Collections.Generic;
using System.Text;

namespace Entities.Enum
{
    public enum OrderStatus
    {
        PendingPayment = 1,
        Paid = 2,
        Shipped = 3,
        Delivered = 4,
        Cancelled = 5,
        Preparing =6
    }
}

