#region License
/*
* Copyright 2013 Weswit Srl
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
* http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*/
#endregion License

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
//using System.Threading;
using System.Timers;
using MarketDataAddin;
using Microsoft.Office.Interop.Excel;

namespace ELFMarketDataLib
{
    /// <summary>
    /// This is the main class of the library and handles the RTD Server and its
    /// communication with Excel, the Lightstreamer Client library and its
    /// communication with Lightstreamer Server.
    /// </summary>
    [ProgId(RtdServer.RTD_PROG_ID), ComVisible(true)]
    //=RTD("MarketData",,"price,IF1308,CCFX")
    public class RtdServer : IRtdServer
    {
        internal const string RTD_PROG_ID = "MarketData";
        private Timer m_timer = new Timer();
        private string m_userId = null;
        private int m_interval = 3000;
        private int m_retried = 0;
        private int serverAlive = 1;

        private DataProvider provider = new DataProvider();
        private Queue<RtdUpdateQueueItem> updateQueue = new Queue<RtdUpdateQueueItem>();

        private int updateQueueMaxLength = 15000;
        private IRTDUpdateEvent rtdUpdateEvent = null;

        private Dictionary<int, string> topicIdMap = new Dictionary<int, string>();
        private Dictionary<string, int> reverseTopicIdMap = new Dictionary<string, int>();

        private LinkedList<string> formulas = new LinkedList<string>();

        private void TimerEventHandler(object sender, EventArgs args)
        {
            bool updatesExist = false;
            foreach (string formula in this.formulas)
            {
                // push to update queue
                if (updateQueue.Count() < updateQueueMaxLength)
                {
                    int topicId;
                    if (reverseTopicIdMap.TryGetValue(formula, out topicId))
                    {
                        updatesExist = true;
                        updateQueue.Enqueue(new RtdUpdateQueueItem(topicId, formula, provider.QueryData(formula)));
                    }
                }
            }
            if (updatesExist && (rtdUpdateEvent != null))
            {
                rtdUpdateEvent.UpdateNotify();
            }
        }
        public Array RefreshData(ref int TopicCount)
        {
            int enqueuedUpdates = updateQueue.Count();
            int updatesCount = 0;
            object[,] data = new object[2, enqueuedUpdates];
            while (enqueuedUpdates > 0)
            {
                RtdUpdateQueueItem item;
                try
                {
                    item = updateQueue.Dequeue();
                }
                catch (InvalidOperationException)
                {
                    break;
                }
                try
                {
                    data[0, updatesCount] = item.TopicID;
                    string value = item.DataValue.ToString();
                    if (value != null || value != "")
                    {
                        data[1, updatesCount] = Double.Parse(value);
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
                ++updatesCount;
                --enqueuedUpdates;
            }
            TopicCount = updatesCount;
            return data;
        }

        public dynamic ConnectData(int TopicID, ref Array Strings, ref bool GetNewValues)
        {
            if (Strings.Length > 0)
            {
                if (m_userId == null)
                {
                    m_userId = provider.Connect();
                }

                string m_itemKey = (string)Strings.GetValue(0);
                if (this.formulas == null)
                {
                    this.formulas = new LinkedList<string>();
                }
                if (!this.formulas.Contains(m_itemKey))
                {
                    this.formulas.AddLast(m_itemKey);
                }
                topicIdMap[TopicID] = m_itemKey;
                reverseTopicIdMap[m_itemKey] = TopicID;
                object value = provider.QueryData(m_itemKey);
                if (value == null)
                {
                    return "Wait...";
                }
                return value;

            }
            return "ERROR";
        }

        public void DisconnectData(int TopicID)
        {
            string data;
            if (topicIdMap.TryGetValue(TopicID, out data))
            {
                reverseTopicIdMap.Remove(data);
            }
            topicIdMap.Remove(TopicID);
        }

        public int Heartbeat()
        {
            return serverAlive;
        }

        public int ServerStart(IRTDUpdateEvent CallbackObject)
        {
            reverseTopicIdMap.Clear();
            updateQueue.Clear();
            topicIdMap.Clear();

            rtdUpdateEvent = CallbackObject;
            System.Threading.ThreadPool.QueueUserWorkItem(
                new System.Threading.WaitCallback(Connect));

            return 1;
        }

        private void Connect(Object obj)
        {
            m_userId = null;

            while (true)
            {
                m_retried++;
                System.Threading.Thread.Sleep(100);
                if (formulas.Count < 1)
                {
                    this.formulas = provider.GetFormula();
                }
                m_userId = provider.Connect();
                if (m_userId != null)
                {
                    this.m_interval = provider.GetInterval();
                    break;
                }
                if (m_retried > 300)
                {
                    this.m_interval = 3000;//default
                    break;
                }
            }
            m_timer.Elapsed += new ElapsedEventHandler(TimerEventHandler);
            m_timer.Interval = this.m_interval;
            m_timer.Start();
        }

        public void ServerTerminate()
        {
            if (null != m_timer)
            {
                m_timer.Dispose();
                m_timer = null;
            }

            if (rtdUpdateEvent != null)
            {
                rtdUpdateEvent.Disconnect();
                rtdUpdateEvent = null;
            }

            updateQueue.Clear();
            topicIdMap.Clear();
            reverseTopicIdMap.Clear();

            m_userId = null;
            provider = null;

        }
    }
}
