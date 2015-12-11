using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NC_TOOL
{
   public static class StaticPara
    {
        private static Dictionary<string, string> fastDic;
       public static Dictionary<string, string> fstenerT
        {
            get
            {
                if(fastDic==null||fastDic.Count()==0)
                {
                    //Get the fastener dictionary


                }
                return fastDic;
            }
            set
            {

            }
        }




    }
}
