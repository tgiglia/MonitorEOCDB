using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitorEOCDB
{
    class DatabaseReplicaStatesRow
    {
        public String replica_id { get; set; }
        public String database_name { get; set; }
        public String replicaName { get; set; }
        public bool is_failover_ready { get; set; }
        public bool is_pending_secondary_suspend { get; set; }
        public bool is_database_joined { get; set; }

        public override string ToString()
        {
            return "replica_id: " + replica_id + "\ndatabase_name: " + database_name + "\nreplicaName: " + replicaName +
                "\nis_fail_ready: " + is_failover_ready +
                "\nis_pending_secondary_suspend: " + is_pending_secondary_suspend + "\nis_database_joined: " + is_database_joined;
        }
    }
}
