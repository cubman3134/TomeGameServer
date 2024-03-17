using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOMEDBDataAccess
{
    public class DatabaseInfo : Interfaces.DatabaseInterface
    {
        private static DatabaseInfo _instance;
        public static DatabaseInfo Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new DatabaseInfo();
                }
                return _instance;
            }
        }

        public override string ConnectionString 
        {
            get
            {
                return "Server=localhost\\SQLEXPRESS;Database=TomeDB;Trusted_Connection=True;";
            }
        }
    }
}
