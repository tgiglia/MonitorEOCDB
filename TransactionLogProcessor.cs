using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;


namespace MonitorEOCDB
{
    class TransactionLogProcessor
    {
        public float logSpaceUsed { get; set; }
        public List<SQLPERFRow> rows { get; }
        public TransactionLogProcessor()
        {
            rows = new List<SQLPERFRow>();
        }
        public bool execSQLPerf(ConfigData cd,String primaryHost)
        {
            
            Logger.logIt(cd, "TransactionLogProcessor::execSQLPerf: trying to connect to: " + cd.dataSource);
            SqlConnection myConnection = new SqlConnection("user id =...;password=...;Integrated Security=SSPI;Data Source=" + cd.dataSource +
                 ";Initial Catalog=master");

            try
            {
                myConnection.Open();
                Logger.logIt(cd, "TransactionLogProcessor::execSQLPerf: Connection has been opened.");
                SqlDataReader reader = null;
                SqlCommand command = new SqlCommand("DBCC SQLPERF (logspace)", myConnection);
                reader = command.ExecuteReader();
                
                Logger.logIt(cd, "TransactionLogProcessor::execSQLPerf: Reading the results.");
                while (reader.Read())
                {
                    SQLPERFRow spr = new SQLPERFRow();
                    spr.databaseName = reader.GetString(0);
                    spr.logSize = reader.GetFloat(1);
                    spr.spaceUsed = reader.GetFloat(2);
                    spr.status = reader.GetInt32(3);
                    rows.Add(spr);
                    //Console.WriteLine(spr.ToString() + "\n");
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                Logger.logIt(cd, e.Message);
                return false;
            }
            return true;
        }
        public bool executeTranLogClear(ConfigData cd, String primaryHost)
        {
            //Console.WriteLine("execSQLPerf: trying to connect to: " + cd.dataSource);
            Logger.logIt(cd, "TransactionLogProcessor::executeTranLogClear: trying to connect to: " + cd.dataSource);
            SqlConnection myConnection = new SqlConnection("user id =...;password=...;Integrated Security=SSPI;Data Source=" + cd.dataSource +
                 ";Initial Catalog=master");
            try
            {
                myConnection.Open();
               
                Logger.logIt(cd, "TransactionLogProcessor::executeTranLogClear: Connection has been opened.");
                SqlCommand command = new SqlCommand("BACKUP LOG EOC_TRAN TO DISK='NUL'", myConnection);
                command.CommandTimeout = cd.querytimeout;
                
                Logger.logIt(cd, "TransactionLogProcessor::executeTranLogClear: Executing clear transaction log command.");
                //int rowsAffected = command.ExecuteNonQuery();
                //Console.WriteLine("Finished Execute transaction log clear. rowsAffected = " + rowsAffected);
                SqlDataReader reader = command.ExecuteReader();
                while (reader.Read()) { }
                
                Logger.logIt(cd, "TransactionLogProcessor::executeTranLogClear: Finished clearing log.");

            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                Logger.logIt(cd, e.Message);
                return false;
            }
            return true;
        }
        public bool analyze(ConfigData cd,String primaryHost)
        {
            //Find the target transaction log
            foreach(SQLPERFRow spr in rows)
            {
                if(spr.databaseName.Equals(cd.trantarget)) {
                    //Console.WriteLine("TransactionLogProcessor::analyze: found target: " + cd.trantarget);
                    logSpaceUsed = spr.spaceUsed;
                    if (spr.spaceUsed > cd.tranthreshold)
                    {
                        //Console.WriteLine("used :" + spr.spaceUsed + " is greater than threshold: " + cd.tranthreshold + " time to flush the log.");
                        Logger.logIt(cd, "used :" + spr.spaceUsed + " is greater than threshold: " + cd.tranthreshold + " time to flush the log.");
                        return true;
                    }
                    else
                    {
                        //Console.WriteLine("used :" + spr.spaceUsed + " is less than threshold: " + cd.tranthreshold + " not time to flush the log.");
                        Logger.logIt(cd, "used :" + spr.spaceUsed + " is less than threshold: " + cd.tranthreshold + " not time to flush the log.");
                    }
                }
            }
            return false;
        }
    }
}
