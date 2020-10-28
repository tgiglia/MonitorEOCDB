using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MonitorEOCDB
{
    class Logger
    {
        public static void logIt(ConfigData cd, String s)
        {
            DateTime localTime = DateTime.Now;
            if (!File.Exists(cd.logfile))
            {
                using (StreamWriter sw = File.CreateText(cd.logfile))
                {

                }
            }
            using (StreamWriter sw = File.AppendText(cd.logfile))
            {
                sw.WriteLine(localTime.ToString() + ":" + s);
            }
        }
        public static void logIt(String filepath, String s)
        {
            DateTime localTime = DateTime.Now;
            if (!File.Exists(filepath))
            {
                using (StreamWriter sw = File.CreateText(filepath))
                {

                }
            }
            using (StreamWriter sw = File.AppendText(filepath))
            {
                sw.WriteLine(localTime.ToString() + ":" + s);
            }
        }

    }
}
