using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enums;

public enum VeilingKlokStatus
{
    Scheduled = 1,
    Started = 2, // Clock is running
    Paused = 3, // Clock is paused (waiting for some action)
    Stopped = 4, // Clock is stopped but not yet ended waiting for product
    Ended = 5 // Completed veiling
}