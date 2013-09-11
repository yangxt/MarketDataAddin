using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MarketDataAddin
{
    class HeartbeatJob: ISchedulerJob
    {
        ConnectionService connect;
        Logger logger;
        public HeartbeatJob()
        {
            connect = new ConnectionService();
            logger = Logger.instance;
        }
        #region ISchedulerJob


        public void Execute()
        {
            try
            {
                logger.WriteInfo("send heartbeat at :" + DateTime.Now);
                connect.Heartbeat();
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
        #endregion
    }
}
