using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.MessagePatterns;

namespace MarketDataAddin
{
    public class SubscriptionService
    {
        private Macro macro = Macro.instance;
        private Logger logger = Logger.instance;
        private Utilities util = Utilities.instance;

        private const string exchange = "elf.server.monitor.control.exchange";
        private const string exchangeType = ExchangeType.Direct;
        private string routingKey = RoutingKey.Subscribe;
        private string queue;

        private static ConnectionFactory cf;
        private static IConnection conn;
        private static IModel channel;
        private static Subscription sub;

        public SubscriptionService()
        {
        }

        public void Add(LinkedList<string> formulas)
        {
            if (macro.userID == null || formulas == null)
            {
                return;
            }
            Initialize();
            using (conn)
            {
                using (channel)
                {
                    SendMessages(formulas);
                    using (sub)
                    {
                        ReceiveMessages(sub);
                    }
                }
            }
        }

        public void Add(string formula)
        {
            if (macro.userID == null || formula == null)
            {
                return;
            }
            Initialize();
            using (conn)
            {
                using (channel)
                {
                    SendMessage(formula);
                    
                    using (sub)
                    {
                        ReceiveMessages(sub);
                    }
                }
            }
        }

        public void Remove(string routingkey)
        {
            //TODO 
        }

        public void Remove()
        {
            //TODO
        }

        private void Initialize()
        {
            queue = macro.userID;

            ConfigQueue();
            EnsureQueue();
        }

        private void ConfigQueue()
        {
            cf = new ConnectionFactory();
            cf.HostName = macro.hostname;
            cf.Port = macro.hostport;
            cf.UserName = macro.username;
            cf.Password = macro.password;
        }
        private void EnsureQueue()
        {
            try
            {
                conn = cf.CreateConnection();
                channel = conn.CreateModel();

                channel.ExchangeDeclare(exchange, exchangeType, true);
                channel.QueueDeclare(queue, false, false, true, null);
                channel.QueueBind(queue, exchange, routingKey, null);

                sub = new Subscription(channel, queue);
            }
            catch (Exception e)
            {
                logger.WriteInfo("Exception From SubscriptionService" + e.Message);
            }
        }

        private void SendMessage(string msg)
        {
            if (macro.userID == null)
            {
                logger.WriteInfo("SubscriptionService sendMessages userID null exit");
                return;
            }
            string message = macro.userID + "|" + msg;
            logger.WriteInfo("sendMessages " + message);
            try
            {
                channel.BasicPublish(exchange, routingKey, null, util.GetBytes(message));
            }
            catch (System.Exception ex)
            {
                logger.WriteInfo("Exception From SubscriptionService" + ex.Message);
            }

        }
        private void SendMessages(LinkedList<string> msgs)
        {
            foreach (string msg in msgs)
            {
                SendMessage(msg);
            }
        }

        private void ReceiveMessages(Subscription sub)
        {
            while (true)
            {
                string msg = messageText(sub.Next());
                logger.WriteInfo("ReceiveMessage " + msg);
                sub.Ack();
                try
                {
                    if (msg.Contains("="))
                    {
                        string[] temp = msg.Split(new char[] { '=' });
                        string key = temp[0];
                        StringBuilder value = new StringBuilder(temp[1]);
                        if (macro.valueDict.ContainsKey(key))
                        {
                            macro.valueDict[key] = value;
                            logger.WriteInfo("Update Value " + value + "of  key" + key);
                        }
                        else
                        {
                            macro.valueDict.Add(key, value);
                            logger.WriteInfo("New Key" + key + " and Value " + value);
                        }
                    }
                }
                catch (Exception e)
                {
                    logger.WriteInfo("Exception From SubscriptionService" + e.Message);
                }
            }
        }
        private string messageText(BasicDeliverEventArgs ev)
        {
            return util.GetString(ev.Body);
        }
    }
}
