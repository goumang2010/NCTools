using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NC_TOOL
{
 public   class Fasterner : IFasterner
    {
    public  string FasternerName
        {
            get;
            set;
        }

        public int Qty
        {
            get; set;
        }

        public   string ResyncCode
        {
            get;
            set;
        }

       

      public  int TCode
        {
            get;
            set;
        }
    }
}
