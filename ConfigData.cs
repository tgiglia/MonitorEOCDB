using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitorEOCDB
{
    class ConfigData
    {
        public String dataSource { get; set; }
        public String dataSource1 { get; set; }
        public String dataSource2 { get; set; }
        public String catalog { get; set; }
        public String emailAddress { get; set; }
        public String emailFrom { get; set; }
        public String emailTo { get; set; }
        public String backlogcatalog { get; set; }
        public String backlogsubscriber { get; set; }
        public String logfile { get; set; }
        public String trantarget { get; set; }
        public int backlogthreshold { get; set; }
        public float tranthreshold { get; set; }
        public int querytimeout { get; set; }
        public int hourtosend { get; set; }
        public bool LoadConfig(System.Diagnostics.EventLog eventLog1)
        {
            try
            {
                dataSource = System.Configuration.ConfigurationManager.AppSettings["dataSource"];
                dataSource1 = System.Configuration.ConfigurationManager.AppSettings["dataSource1"];
                dataSource2 = System.Configuration.ConfigurationManager.AppSettings["dataSource2"];
                catalog = System.Configuration.ConfigurationManager.AppSettings["catalog"];
                emailAddress = System.Configuration.ConfigurationManager.AppSettings["emailAddress"];
                emailFrom = System.Configuration.ConfigurationManager.AppSettings["emailFrom"];
                emailTo = System.Configuration.ConfigurationManager.AppSettings["emailTo"];
                backlogcatalog = System.Configuration.ConfigurationManager.AppSettings["backlogcatalog"];
                backlogsubscriber = System.Configuration.ConfigurationManager.AppSettings["backlogsubscriber"];
                logfile = System.Configuration.ConfigurationManager.AppSettings["logfile"];
                String tmp = System.Configuration.ConfigurationManager.AppSettings["backlogthreshold"];
                backlogthreshold = Int32.Parse(tmp);
                trantarget = System.Configuration.ConfigurationManager.AppSettings["trantarget"];
                tmp = System.Configuration.ConfigurationManager.AppSettings["tranthreshold"];
                tranthreshold = float.Parse(tmp);
                tmp = System.Configuration.ConfigurationManager.AppSettings["querytimeout"];
                querytimeout = Int32.Parse(tmp);
                tmp = System.Configuration.ConfigurationManager.AppSettings["hourtosend"];
                hourtosend = Int32.Parse(tmp);

            }
            catch (Exception e)
            {
                eventLog1.WriteEntry(e.Message);
                return false;
            }

            return true;
        }
        public override string ToString()
        {
            return "dataSource: " + dataSource + "\ndataSource1: " + dataSource1 + "\ndataSource2: " + dataSource2 +
                "\ncatalog: " + catalog + "\nbacklogcatalog: " + backlogcatalog +
                "\nemailAddress: " + emailAddress +
                "\nemailFrom: " + emailFrom + "\nemailTo: " + emailTo +
                "\nbacklogthreshold: " + backlogthreshold +
                "\nbacklogsubscriber: " + backlogsubscriber + "\nlogfile: " + logfile +
                "\ntrantarget: " + trantarget +
                "\ntranthreshold: " + tranthreshold +
                "\nquerytimeout: " + querytimeout +
                "\nhourtosend: " + hourtosend;
         }
    }
}

