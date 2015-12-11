using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NC_TOOL
{
 public   interface IDBInfo
    {
        FasternersRepo DBfstTable { get; set; }
        IProduct DBproduct { get; set; }
        IDictionary<string, int> DBfastlist { get; set; }
        IDictionary<string, int> DBdrilllist { get; set; }

    }
}
