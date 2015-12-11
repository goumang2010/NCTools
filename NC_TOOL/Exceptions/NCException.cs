using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NC_TOOL
{
  public  class NCException : ApplicationException
    {
      
        public NCException() { }
        public NCException(string message)  : base(message) { }
        public NCException(string message, Exception inner) : base(message, inner) { }




    }
}
