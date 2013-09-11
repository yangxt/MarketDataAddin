using System.Text;
using System.Diagnostics;
using System.Collections.Generic;

namespace MarketDataAddin
{
    public class DataProvider
    {
        private Logger logger = Logger.instance;
        private Macro macro = Macro.instance;

        private MarketDataProxy proxy = new MarketDataProxy();

        public string Connect()
        {
            return macro.userID;
        }

        public LinkedList<string> GetFormula()
        {
            return macro.formulas;
        }

        public int GetInterval()
        {
            return macro.interval;
        }

        //itemKey PRICE,IF1308,CCFX
        //return 25789.99
        public object QueryData(string itemKey)
        {
            object rt = null;
            if (itemKey == null || macro.valueDict == null)
            {
                rt = "N/A";
            }
            string queryKey = GetFormula(itemKey);
            Debug.WriteLine("QueryData " + queryKey);
            if (macro.valueDict.ContainsKey(queryKey))
            {
                rt = macro.valueDict[queryKey];
            }
            else
            {
                macro.valueDict.Add(queryKey, new StringBuilder());
                proxy.Subscription(queryKey);
            }
            return rt;
        }
        //subKey PRICE,IF1308,CCFX
        //formula MarketData("PRICE","IF1308","CCFX")
        private string GetFormula(string itemKey)
        {
            string[] pramas = itemKey.Split(new char[] { ',' });
            string formula = "MarketData(\"" + pramas[0] + "\",\"" + pramas[1] + "\",\"" + pramas[2] + "\")";
            return formula;
        }
    }
}
