using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace MonitorEOCDB
{
    class Program
    {
        static void Main(string[] args)
        {
            EventLog eventLog1 = new EventLog("Application");
            eventLog1.Source = "Application";
                       
            ConfigData cd = new ConfigData();
            if(!cd.LoadConfig(eventLog1))
            {
                return;
            }
            //Console.WriteLine(cd.ToString());

            Logger.logIt(cd,cd.ToString());
            DatabaseReplicateStatesProcessor drsp = new DatabaseReplicateStatesProcessor();
            bool bRt = drsp.executeSQL(cd);
            if (bRt)
            {
                bool bFailure = drsp.analyze(cd);
                if (bFailure)
                {
                    EmailNotification en = new EmailNotification();
                    en.sendEmail(cd, drsp.sb.ToString(), "AOAG Sync State Error");
                    Logger.logIt(cd, drsp.sb.ToString());
                    
                }
            }
            String replica = drsp.getPrimaryNodeId(cd);
            if(replica != null)
            {
                String primaryHost = drsp.matchReplicaIdToHost(cd, replica);
                if(primaryHost != null)
                {
                    Console.WriteLine("The primary host is: " + primaryHost);
                    TransactionLogProcessor tlp = new TransactionLogProcessor();
                    bool bTran = tlp.execSQLPerf(cd, primaryHost);
                    if(bTran)
                    {
                        bool timeToFlush = tlp.analyze(cd, primaryHost);
                        //If time to flush, execute the clear long command
                        if(timeToFlush)
                        {
                            Console.WriteLine("Its time to clear the log.");
                            tlp.executeTranLogClear(cd, primaryHost);
                        }


                    }
                    else
                    {
                        Logger.logIt(cd,"ERROR. TransactionLogProcessor::execSQLPerf failed.");
                    }
                }
                else
                {
                    Console.WriteLine("Could not find primary host.");
                }
            }
        }
    }

}
