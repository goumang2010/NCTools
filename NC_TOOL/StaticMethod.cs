using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NC_TOOL
{
   public static class StaticMethod
    {

     public static void Merge(this  Dictionary<string,int > fastlist,string fastname)
        {
            if (fastlist.Keys.Contains(fastname))
            {
                fastlist[fastname] = fastlist[fastname] + 1;

            }
            else
            {
                fastlist.Add(fastname, 1);

            }


        }

        public static bool Contains(this List<NCpointCoord> nclist,string uuid)
        {

            var lq = (from pp in nclist
                      where pp.UUID == uuid
                      select pp);
            return (lq.Count() > 0);

        }




    }
}
