using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MarketDataAddin
{
    interface ISchedulerJob
    {
        void Execute();
    }
}
