using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NC_TOOL
{
  public  class ProductInfo:IProduct
    {

        //public string ProductName
        //{
        //    get;
        //    set;
        //}
        public  string ProgramNum
        {
            get;
            set;
        }

        public string ProductEngName
        {
            get;

        }
        public string ProductChnName
        {
            get;
            set;
        }

       public string CurrentBatch
        {
            get;
            set;
        }

        public string StationNum
        {
            get;
            set;
        }

        public string ProductNum
        {
            get;
            set;

          
        }

        public string ProcessPath
        {
            get;
            
            
        }

        public string TVApath
        {
            get;
            
        }



    }
}
