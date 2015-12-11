using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace NC_TOOL
{
  public  class FasternersRepo : List<IFasterner>
    {

        Func<IEnumerator, dynamic> allreturn = delegate (IEnumerator targetlist)
          {
              targetlist.MoveNext();
              if (targetlist.Current==null)
              {
                  throw new NCException("没有这种紧固件！");
                 
              }
              else
              {
                  
                  return targetlist.Current;
              }
          };


        public IFasterner getFasterner(string FastName)
        {
            return allreturn((from pp in this
                              where pp.FasternerName == FastName
                              select pp).GetEnumerator());
          
        }
        public string getFastName(int Tcode)
        {
         return  allreturn((from pp in this
                     where pp.TCode== Tcode
                     select pp.FasternerName).GetEnumerator());
          
            
        }

        public int getQTY(string FastName)
        {
            return allreturn((from pp in this
                             where pp.FasternerName == FastName
                             select pp.Qty).GetEnumerator());
        }



        public string getResyncCode(string FastName)
        {
            return allreturn((from pp in this
                             where pp.FasternerName == FastName
                             select pp.ResyncCode).GetEnumerator());

        }

        public int? getTCode(string FastName)
        {
            return allreturn((from pp in this
                             where pp.FasternerName == FastName
                             select pp.TCode).GetEnumerator());
        }

        public IEnumerable<string> GetList(string fieldName)
        {
            Type type =typeof(Fasterner);
           var field = type.GetProperty(fieldName);
            foreach(Fasterner ff in this)
           {
             yield return  (string)field.GetValue(ff,null);

            }

        }

        public  void Merge(IFasterner fst)
        {
            var nameLs = GetList("FasternerName");
            if(nameLs.Contains(fst.FasternerName))
            {
                getFasterner(fst.FasternerName).Qty += 1;
            }
            else
            {
                this.Add(fst);
            }

        }

        public void Merge(string FastName, int qty=1)
        {
            var nameLs = GetList("FasternerName");
            if (nameLs.Contains(FastName))
            {
                getFasterner(FastName).Qty += qty;
            }
            else
            {
                this.Add(new Fasterner() { FasternerName= FastName ,Qty= qty });
            }
        }
    }
}
