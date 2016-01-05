using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GoumangToolKit;

namespace NC_TOOL
{
    public class DBInfo : IDBInfo
    {

      public  DBInfo()
        {
            var duizhao = DbHelperSQL.Query("select  Fasteners,Tcode,Resync_Target from 紧固件列表").Tables[0];
            int count = duizhao.Rows.Count;
            DBfstTable = new FasternersRepo();
            for (int i = 0; i < count; i++)
            {
                string Tcode = duizhao.Rows[i][1].ToString().Replace("T", "");
                DBfstTable.Add(new Fasterner() { FasternerName = duizhao.Rows[i][0].ToString(), TCode = System.Convert.ToInt16(Tcode), ResyncCode = duizhao.Rows[i][2].ToString() });
            }
        }
        public IDictionary<string, int> DBdrilllist
        {
            get;
            set;
        }

        public IDictionary<string, int> DBfastlist
        {
            get;
            set;

        }

        public FasternersRepo DBfstTable
        {
            get;
            set;
        }

        public IProduct DBproduct
        {
            get;
            set;
        }
    }
}
