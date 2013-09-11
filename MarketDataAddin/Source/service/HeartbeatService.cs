using System;
using System.Timers;
using RabbitMQ.Client;
using System.Diagnostics;
using RabbitMQ.Client.MessagePatterns;

namespace MarketDataAddin
{
    class HeartbeatService
    {
        private Logger logger = Logger.instance;
        private Macro macro = Macro.instance;
        private Utilities util = Utilities.instance;

        private Timer m_timer = new Timer();

        private const string exchange = "elf.server.monitor.control.exchange";
        private const string exchangeType = ExchangeType.Direct;
        private string routingKey = RoutingKey.Heartbeat;
        private string queue = "elf.server.monitor.control.heartbeat";

        private static ConnectionFactory cf;
        private static IConnection conn;
        private static IModel channel;
        private static Subscription sub;

        public HeartbeatService()
        {
            
        }

        public void Start()
        {
            m_timer.Enabled = true;
            m_timer.Elapsed += new ElapsedEventHandler(TimerEventHandler);
            m_timer.Interval = 3000;
            m_timer.Start();
        }

        public void Stop()
        {
            if (m_timer == null)
            {
                return;
            }
            m_timer.Dispose();
            m_timer = null;
            sub = null;
            channel = null;
            conn = null;
            cf = null;
        }

        private void ConfigQueue()
        {
            cf = new ConnectionFactory();
            cf.HostName = macro.hostname;
            cf.Port = macro.hostport;
            cf.UserName = macro.username;
            cf.Password = macro.password;

            conn = cf.CreateConnection();
            channel = conn.CreateModel();

        }    
        private void EnsureQueue()
        {
            channel.ExchangeDeclare(exchange, exchangeType, true);
            channel.QueueDeclare(queue, true, false, false, null);
            channel.QueueBind(queue, exchange, routingKey, null);

            sub = new Subscription(channel, queue);
        }

        private void TimerEventHandler(object sender, EventArgs args)
        {
            Heartbeat();
        }

        private void Heartbeat()
        {
            try
            {
                ConfigQueue();
                using (conn)
                {
                    using (channel)
                    {
                        EnsureQueue();

                        SendMessages();
                        
                        using (sub)
                        {
                            ReceiveMessages(sub);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                logger.WriteInfo(e.Message);
            }
        }
        private void SendMessages()
        {
            try
            {
                Debug.WriteLine("Sending Heartbeat messages " + exchange + " " + routingKey + "  " + queue);
                IBasicProperties basicProperties = channel.CreateBasicProperties();
                channel.BasicPublish(exchange, routingKey, false, false, basicProperties, util.GetBytes(macro.userID));
            }
            catch (Exception e)
            {
                logger.WriteInfo(e.Message);
            }
        }
        private void ReceiveMessages(Subscription sub)
        {
                sub.Next();
                sub.Ack();
        }
    }
}
