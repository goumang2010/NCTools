using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NC_TOOL
{
   public interface IProduct
    {
        string ProductNum { get; }
        string ProductEngName { get; }
        string ProductChnName { get; }
        string CurrentBatch { get; }

        string StationNum { get; }
        string ProcessPath { get; }
        string TVApath { get; }








    }
}
