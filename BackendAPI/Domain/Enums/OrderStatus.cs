using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enums
{
    public enum OrderStatus
    {
        Open = 0,
        Processing = 1,
        Processed = 2,
        Delivered = 3,
        Cancelled = 4,
        Returned = 5,
    }
}
