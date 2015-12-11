using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NC_TOOL
{
  public  interface IFasterner
    {
        string FasternerName
        {
            get;
        }

       int TCode
        {
            get;
        }

        string ResyncCode
        {
            get;
        }

        int Qty { get; set; }
    }
}
