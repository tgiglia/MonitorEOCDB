using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonitorEOCDB
{
    class SQLPERFRow
    {
        public String databaseName { get; set; }
        public float logSize { get; set; }
        public float spaceUsed { get; set; }
        public int status { get; set; }
        public override string ToString()
        {
            return "databaseName: " + databaseName + "\nlogSize: " + logSize + "\nspaceUsed: " + spaceUsed + "\nstatus: " + status;
        }
    }
}
