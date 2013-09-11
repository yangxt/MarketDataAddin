using System;
using System.Text;
using System.Threading;

namespace MarketDataAddin
{
    public class MarketDataProxy 
    {
        private Macro macro = Macro.instance;
        private Logger logger = Logger.instance;
        private Utilities util = Utilities.instance;

        ConfigrationService configrationService = new ConfigrationService();
        SubscriptionService subscriptionService = new SubscriptionService();
        HeartbeatService heartbeatService = new HeartbeatService();
        ConnectionService connectionService = new ConnectionService();
        FormulaReader formulaReader = new FormulaReader();

        public void initService()
        {
            ReadConfig();
            ReadFormula();
            OpenQueue();
            ForTest();
        }
        public void startService()
        {
            ThreadPool.QueueUserWorkItem(new WaitCallback(HeartbeatThread));
            ThreadPool.QueueUserWorkItem(new WaitCallback(SubscribeThread));
        }
        public void stopService()
        {
            heartbeatService.Stop();
            subscriptionService.Remove();
        }

        public void Subscription(string formula)
        {
            macro.formulas.AddLast(formula);
            ThreadPool.QueueUserWorkItem(new WaitCallback(AddSubThread), formula);
        }
        private void AddSubThread(Object obj)
        {
            subscriptionService.Add((string)obj);
        }
        private void SubscribeThread(Object obj)
        {
            subscriptionService.Add(macro.formulas); 
        }
        private void HeartbeatThread(object obj)
        {
            heartbeatService.Start();
        }

        private void OpenQueue()
        {
            connectionService.Open();
        }
        private void ReadConfig()
        {
            configrationService.readXML();
        }
        private void ReadFormula()
        {
            formulaReader.ReadFormula();
        }

        private void ForTest()
        {
            //For Test 
            for (int i = 1308; i < 1500; i++)
            {
                string tmp = "MarketData(\"price\",\"IF" + i.ToString() + "\",\"CCFX\")";
                macro.formulas.AddLast(tmp);
                tmp = null;
            }
            //For Test
            for (int i = 1308; i < 1500; i++)
            {
                string tmp = "MarketData(\"price\",\"IF" + i.ToString() + "\",\"CCFX\")";
                string value = new Random().NextDouble().ToString();
                macro.valueDict.Add(tmp, new StringBuilder(value));
                tmp = null;
                value = null;
            }
        }
    }
}
