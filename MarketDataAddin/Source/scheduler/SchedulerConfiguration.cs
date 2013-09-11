using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;

namespace MarketDataAddin
{
    class SchedulerConfiguration
    {
        private int sleepInterval;
        private ArrayList jobs = new ArrayList();

        public int SleepInterval { get { return sleepInterval; } }
        public ArrayList Jobs { get { return jobs; } }


        public SchedulerConfiguration(int newSleepInterval)
        {
            sleepInterval = newSleepInterval;
        }
    }
}
