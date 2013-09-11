using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ELFMarketDataLib
{
    class DataListener
    {
        private IRtdListener listener;

        public DataListener(IRtdListener listener)
        {
            this.listener = listener;
        }

        public DataListener()
        {
            // TODO: Complete member initialization
        }

        public void Update()
        {
            listener.OnItemUpdate(itemPos, itemName, update);
        }

    }
}
