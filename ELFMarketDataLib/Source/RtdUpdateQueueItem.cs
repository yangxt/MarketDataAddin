using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ELFMarketDataLib
{
    class RtdUpdateQueueItem
    {
        public int TopicID;
        public string ItemName;
        public object DataValue;
        public RtdUpdateQueueItem(int TopicID, string ItemName, object DataValue)
        {
            this.TopicID = TopicID;
            this.ItemName = ItemName;
            this.DataValue = DataValue;
        }
    }
}
