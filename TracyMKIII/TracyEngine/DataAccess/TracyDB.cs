using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tracy.DataAccess
{
    class TracyDB : MongoDB
    {
        public override string ConnectionString
        {
            get
            {
                return "mongodb://localhost:27017";
            }
        }

        public override string DBName
        {
            get
            {
                return "tracy";
            }
        }
    }
}
