using System;
using System.IO;
using System.Diagnostics;

namespace MarketDataAddin
{
    public class Logger
    {
        private const string LogPath = @"D:\HSETFAddin.log";
        private StreamWriter log;
        public static readonly Logger instance = new Logger();

        private Logger()
        {
            log = new StreamWriter(LogPath, true);
        }

        public void WriteInfo(string message)
        {
            WriteInfo("{0}", message);
        }
        public void WriteInfo(string format, params object[] obj)
        {
            try
            {
                string msg = string.Format("[{0}] {1}", System.DateTime.Now, string.Format(format, obj));
                if (int.Parse(Macro.instance.debug) == 0)
                {
                    Debug.WriteLine(msg);
                }
                else
                {
                    log.WriteLine(msg);
                    log.Flush();
                }
            }
            catch(Exception e)
            {
                Debug.WriteLine(e.Message);
            }
        }
    }
}
