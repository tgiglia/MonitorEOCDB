using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace MonitorEOCDB
{
    class DatabaseReplicateStatesProcessor
    {
        //String sql = "select replica_id,database_name,is_failover_ready,is_pending_secondary_suspend,is_database_joined from sys.dm_hadr_database_replica_cluster_states";
        String sqlJoin = "select a.replica_id,database_name,is_failover_ready,is_pending_secondary_suspend,is_database_joined," +
            "replica_server_name from sys.dm_hadr_database_replica_cluster_states a " +
            "INNER JOIN sys.dm_hadr_availability_replica_cluster_states b " +
            "ON a.replica_id = b.replica_id";


        List<DatabaseReplicaStatesRow> rows { get; }
        public StringBuilder sb { get; set; }
        public DatabaseReplicateStatesProcessor()
        {
            rows = new List<DatabaseReplicaStatesRow>();
        }
        public bool executeSQL(ConfigData cd)
        {
            Console.WriteLine("trying to connect to: " + cd.dataSource);
            SqlConnection myConnection = new SqlConnection("user id =...;password=...;Integrated Security=SSPI;Data Source=" + cd.dataSource +
                 ";Initial Catalog=" + cd.catalog);
            try
            {
                myConnection.Open();
                Logger.logIt(cd, "DatabaseReplicateStatesProcessor: Connection has been opened.");
                SqlDataReader reader = null;
                SqlCommand command = new SqlCommand(sqlJoin, myConnection);
                reader = command.ExecuteReader();
                Logger.logIt(cd, "DatabaseReplicateStatesProcessor: Reading the results.");             
                while (reader.Read())
                {
                    DatabaseReplicaStatesRow r = new DatabaseReplicaStatesRow();
                    r.replica_id = reader.GetGuid(0).ToString();
                    r.database_name = reader.GetString(1);
                    r.is_failover_ready = reader.GetBoolean(2);
                    r.is_pending_secondary_suspend = reader.GetBoolean(3);
                    r.is_database_joined = reader.GetBoolean(4);
                    r.replicaName = reader.GetString(5);
                    rows.Add(r);
                    
                    Logger.logIt(cd, r.ToString());
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Logger.logIt(cd, e.Message);
                return false;
               
            }
            return true;
        }
        public String getPrimaryNodeId(ConfigData cd)
        {
            SqlConnection myConnection = new SqlConnection("user id =...;password=...;Integrated Security=SSPI;Data Source=" + cd.dataSource +
                 ";Initial Catalog=" + cd.catalog);
            String replica = null;
            try
            {
                String cmd = "select replica_id from sys.dm_hadr_database_replica_states where is_primary_replica = 1";
                myConnection.Open();
                Logger.logIt(cd, "getPrimaryNodeId: Connection has been opened.");
                SqlDataReader reader = null;
                SqlCommand command = new SqlCommand(cmd, myConnection);
                reader = command.ExecuteReader();
                Logger.logIt(cd, "getPrimaryNodeId: Reading the results.");
                while (reader.Read())
                {
                    replica = reader.GetGuid(0).ToString();
                    Console.WriteLine("getPrimaryNodeId: " + replica);
                }

            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                Logger.logIt(cd, e.Message);
            }
            return replica;
        }
        public String matchReplicaIdToHost(ConfigData cd, String id)
        {
            String primaryHost = null;
            foreach(DatabaseReplicaStatesRow r in rows)
            {
                if(r.replica_id.Equals(id))
                {
                    primaryHost = r.replicaName;
                }
            }
            return primaryHost;
        }
        public bool analyze(ConfigData cd)
        {
            bool rt = false;
            sb = new StringBuilder();           
            Logger.logIt(cd,"DatabaseReplicateStateProcesseor::analyze: There are " + rows.Count + " to analyze.");
            foreach (DatabaseReplicaStatesRow r in rows)
            {
                if (analyzeRow(cd,r, sb))
                {
                    rt = true;
                }
            }
            if (rt)
            {
                Logger.logIt(cd,sb.ToString());              
            }
            else
            {
                Logger.logIt(cd, rows[0].database_name + " AOAG is stable.");
            }
            return rt;
        }
        private bool analyzeRow(ConfigData cd, DatabaseReplicaStatesRow r, in StringBuilder sb)
        {
            bool rt = false;

            if (r.is_failover_ready == false)
            {
                rt = true;
                sb.Append("Database: " + r.replicaName + " FAIL: Database is not synchronized.\n");
            }
            if (r.is_database_joined == false)
            {
                rt = true;
                sb.Append("Database: " + r.replicaName + " FAIL: Database is not joined to availability group.\n");
            }
            return rt;
        }

    }
}

