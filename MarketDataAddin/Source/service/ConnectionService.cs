using System;
using RabbitMQ.Client;
using RabbitMQ.Client.MessagePatterns;
using System.Windows.Forms;

namespace MarketDataAddin
{
    class ConnectionService
    {
        private int count = 0;

        private const string exchange = "elf.server.monitor.control.exchange";
        private const string exchangeType = ExchangeType.Direct;
        private const string routingKey = RoutingKey.Open;

        private Logger logger = Logger.instance;
        private Macro macro = Macro.instance;
        private Utilities util = Utilities.instance;

        private static ConnectionFactory cf = new ConnectionFactory();

        public ConnectionService()
        {
            
        }

        private void Initialize()
        {
            try
            {
                cf.HostName = macro.hostname;
                cf.Port = macro.hostport;
                cf.UserName = macro.username;
                cf.Password = macro.password;
                cf.RequestedHeartbeat = 0;
            }
            catch (Exception e)
            {
                logger.WriteInfo("Exception" + e.Message);
            }
        }
        public void Open()
        {
            MessageBox.Show("method open run");
            Initialize();
            try
            {
                using (IConnection conn = cf.CreateConnection())
                {
                    using (IModel channel = conn.CreateModel())
                    {
                        SimpleRpcClient client = new SimpleRpcClient(channel, exchange, exchangeType, routingKey);
                        byte[] requestMessageBytes = util.GetBytes("");
                        while (true)
                        {
                            byte[] replyMessageBytes = client.Call(requestMessageBytes);
                            count = count + 1;
                            if (replyMessageBytes != null)
                            {
                                macro.userID = util.GetString(replyMessageBytes);
                                logger.WriteInfo(string.Format("ConnectionService Open() queueName {0}", macro.userID));
                                break;
                            }
                            System.Threading.Thread.Sleep(100);
                            if (count > 300)
                            {
                                break;
                            }
                        }
                        channel.Close();
                    }
                    conn.Close();
                }
            }
            catch (Exception e)
            {
                logger.WriteInfo(e.Message);
            }
        }
    }
}
