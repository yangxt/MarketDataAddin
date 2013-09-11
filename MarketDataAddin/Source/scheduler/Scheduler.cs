using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;

namespace MarketDataAddin
{
    class Scheduler
    {
        private SchedulerConfiguration configuration = null;
        public Scheduler(SchedulerConfiguration config)
        {
            configuration = config;
        }
        public void Start()
        {
            while (true)
            {
                if (configuration.Jobs.Count < 1) continue;
                foreach (ISchedulerJob job in configuration.Jobs)
                {
                    ThreadStart myThreadDelegate = new ThreadStart(job.Execute);
                    Thread myThread = new Thread(myThreadDelegate);
                    myThread.SetApartmentState(ApartmentState.STA);
                    myThread.Start();
                    Thread.Sleep(configuration.SleepInterval);
                    //Debug.WriteLine("myThread State :" + myThread.ThreadState);
                }
            }
        }
    }
}
