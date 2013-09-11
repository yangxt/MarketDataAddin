using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ELFMarketDataLib
{
    interface IRtdListener
    {
        void OnItemUpdate(int itemPos, string itemName, objec update);
    }
}
