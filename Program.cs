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
            bool bFailure = false;
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
                bFailure = drsp.analyze(cd);
                if (bFailure)
                {
                    EmailNotification en = new EmailNotification();
                    en.sendEmail(cd, drsp.sb.ToString(), "AOAG Sync State Error");
                    Logger.logIt(cd, drsp.sb.ToString());
                    
                }
                else
                {
                    Logger.logIt(cd, "AOAG is STABLE");
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
                            Logger.logIt(cd,"Its time to clear the log.");
                            EmailNotification en = new EmailNotification();
                            bool bSendRt = en.sendEmail(cd,cd.dataSource + " transaction log needs to be cleared.", "CLEAR TRANACTION LOG");
                            if(bSendRt)
                            {
                                Logger.logIt(cd, "Send Email returned successfully.");
                            }
                            else
                            {
                                Logger.logIt(cd,"Send Email FAILED.");
                            }
                            tlp.executeTranLogClear(cd, primaryHost);
                        }
                        else
                        {
                            //Check the time to see if we should send a status email
                            DateTime localTime = DateTime.Now;
                            Logger.logIt(cd, "Current hour: " + localTime.Hour);
                            if(localTime.Hour == cd.hourtosend)
                            {
                                EmailNotification en = new EmailNotification();
                                
                                en.sendEmail(cd, cd.dataSource + " MonitorEOCDB running, no alerts at this time. Log space used: " + tlp.logSpaceUsed +
                                    " . AOAG failure state: " + bFailure,
                                    "Status Message");
                            }
                        }


                    }
                    else
                    {
                        Logger.logIt(cd,"ERROR. TransactionLogProcessor::execSQLPerf failed.");
                        EmailNotification en = new EmailNotification();
                        en.sendEmail(cd, cd.dataSource + " ERROR. TransactionLogProcessor::execSQLPerf failed.","Error Notification");
                    }
                }
                else
                {
                    Logger.logIt(cd, "Could not find primary host.");
                    EmailNotification en = new EmailNotification();
                    en.sendEmail(cd, cd.dataSource + " Could not find primary host.", "Error Notification");
                }
            }
        }
    }

}
