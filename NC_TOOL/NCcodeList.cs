using mysqlsolution;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;


namespace NC_TOOL
{
    public  class NCcodeList:List<string>
    {
        private IDBInfo dbinfo;

        //property

        //the installed fasterners this NC codes covers
        public Dictionary<string, int> fastList { get; set; }
        public Dictionary<string, int> drillList { get; set; }
        public Dictionary<string, int> wronglist { get; set; }
       
        public    NCcodeList(IDBInfo dbInfo)
        {
            dbinfo = dbInfo;
            wronglist = new Dictionary<string, int>();
        }


        private NCpointCoord CreatPointInfo(string[] instcoodrow, int pp)
        {

          
            //get the geoset
            int geosetindex = this.FindLastIndex(pp - 1,s=> s.ToUpper().Contains("START GEOSET"));
            string geosetstr = this.ElementAt(geosetindex).Split(':')[1];
            geosetstr = geosetstr.Remove(geosetstr.Length - 1);
            geosetstr = geosetstr.Trim();

            //get the operation
            int opindex = this.FindLastIndex(pp - 1, s=> s.ToUpper().Contains("START OPERATION"));
            string opstr = this.ElementAt(opindex).Split(':')[1];
            opstr = opstr.Remove(opstr.Length - 1);
            opstr = opstr.Trim();
            var pf = instcoodrow[1].Replace(")", "");
            string[] pfname;
            if (pf.Contains("."))
            {
                pfname = pf.Split('.');
              
            }
            else
            {
                pfname = pf.Split('_');

            }



            NCpointCoord pt = new NCpointCoord(instcoodrow[0].Trim())
            {
                PFName = pfname[0].Trim(),
                PFNum = System.Convert.ToInt32(pf[1]),
                Geoset = geosetstr,
                Operation = opstr,
                RowNum= pp


            };
            return pt;

        }

      public List<NCpointCoord> CheckPoints (Predicate<string> Moperation)
        {

            var fstenerT = dbinfo.DBfstTable;

            int McodeRow = this.FindIndex(0, Moperation);
            int CoordRow = 0;
            var installlistfull = new List<NCpointCoord>();
            int uid = 0;

            while (McodeRow > 0)
            {
                CoordRow = this.FindLastIndex(McodeRow - 1, s => (s.Contains("X") && s.Contains("Y") && s.Contains("Z")));
                //Split the coord and pf name
                string[] instcoodrow = this.ElementAt(CoordRow).Split('(');
                //Create coord infomation
                var instcood = CreatPointInfo(instcoodrow, CoordRow);

                string Tcode;
                int TcodeRow = this.FindLastIndex(McodeRow - 1, s => s.Contains("M56"));
                if (TcodeRow < 0)
                {
                    throw new NCException("这不是完整的程序，没有包含选钉代码");
                }
                else
                {
                    Tcode = this[TcodeRow].Replace("M56T", "");
                }
                //Get the fasterner name per dictionary
                string fstnameT = fstenerT.getFastName(System.Convert.ToInt16(Tcode));
                if (fstnameT != instcood.PFName)
                {
                    wronglist["ProcessFeature名称错误,SX_CENIT," + instcood.OutPut()]= TcodeRow;
                    // throw new NCException(shuchu);
                }
                instcood.PFName = fstnameT;
                //Remove the number after the dot,make the coord integer
                string instcoodsimple2 = instcood.UUID;
             var lastdupliptRow=  installlistfull.FindIndex(p => p.UUID == instcood.UUID);
               
                if (lastdupliptRow != -1 )
                {
                    var lastduplipt = installlistfull[lastdupliptRow];
                    //If the duplicate is a neighbored one,then judge it by A axis, they are only different on A Axis,then they may be ring points not the duplicates
                    if (installlistfull.IndexOf(lastduplipt) == installlistfull.Count - 1)
                    {
                        double mm1 = System.Convert.ToDouble(lastduplipt.A);
                        double mm2 = System.Convert.ToDouble(instcood.A);
                        if (Math.Abs(mm1 - mm2) < 0.4)
                        {

                            wronglist["A坐标相近," + uid + "," + lastduplipt.OutPut()]= lastdupliptRow;
                            wronglist["A坐标相近," + uid + "," + instcood.OutPut()] = CoordRow;
                        }

                    }
                    else
                    {
                        wronglist["重复点," + uid + "," + lastduplipt.OutPut()] = lastdupliptRow;
                        wronglist["重复点," + uid + "," + instcood.OutPut()] = CoordRow;
                    }
                    //Increase the wrong row num
                    uid = uid + 1;
                }
                else
                {
                    //If there is no duplicate point,then add it to the set
                    installlistfull.Add(instcood);
                }
                McodeRow = this.FindIndex(McodeRow + 1, Moperation);

            }
            return installlistfull;
        }
      public string Check(bool erroroutput = true,bool report=false,bool productref=true)
        {

            wronglist = new Dictionary<string, int> ();
           
            var installlistfull = CheckPoints(x => x.Contains("M60") || x.Contains("M62"));
            var drilllistfull = CheckPoints(x => x.Contains("M61") || x.Contains("M63"));
            if (report)
            {
             
                DataTable dt = new DataTable();
                dt.Columns.Add("坐标Coord", typeof(string));
                dt.Columns.Add("紧固件Fastener", typeof(string));
                dt.Columns.Add("PF index", typeof(string));
                dt.Columns.Add("Operation name", typeof(string));
                dt.Columns.Add("Geoset name", typeof(string));
                dt.Columns.Add("程序行号Program row", typeof(string));

                foreach (var item in installlistfull)
                {
                    dt.Rows.Add(item.ToArray());
                }
                if (dt.Rows.Count > 0)
                {
                    OFFICE_Method.excelMethod.SaveDataTableToExcel(dt);
                }
            }
            if (erroroutput)
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("错误类型", typeof(string));
                dt.Columns.Add("UID", typeof(string));
                dt.Columns.Add("坐标Coord", typeof(string));
                dt.Columns.Add("紧固件Fastener", typeof(string));
                dt.Columns.Add("PF index", typeof(string));
                dt.Columns.Add("Operation name", typeof(string));
                dt.Columns.Add("Geoset name", typeof(string));
                dt.Columns.Add("程序行号Program row", typeof(string));

                foreach (string item in wronglist.Keys)
                {
                    dt.Rows.Add(item.Split(','));
                }
                if (dt.Rows.Count > 0)
                {
                    OFFICE_Method.excelMethod.SaveDataTableToExcel(dt);
                }
            }


           int fstqty=   installlistfull.Count();
            int drlqty = drilllistfull.Count();

            var installsta = from pp in installlistfull
                             group pp by pp.PFName into dcol
                             select new
                             {
                                 Key = dcol.Key,
                                 Value = dcol.Count()
                             };

            var drillsta= from pp in drilllistfull
                          group pp by pp.PFName into dcol
                          select new
                          {
                              Key = dcol.Key,
                              Value = dcol.Count()
                          };


            if (productref)
            {
                var allfastlist = dbinfo.DBfastlist;
                var alldrilllist = dbinfo.DBdrilllist;
                int tvaqty = allfastlist.Sum(x => x.Value);
                int tvadrillqty = alldrilllist.Sum(x => x.Value);
                string display = "NC代码紧固件数量：\r\n";
            

               
                foreach (var item in installsta)
                {
                        string TVAqty="";
                        if(allfastlist.Keys.Contains(item.Key))
                        {
                            TVAqty = allfastlist[item.Key].ToString();
                        display = display + item.Key.ToString() + "： " + item.Value.ToString() + " TVA:" + TVAqty + "\r\n";

                    }
                    else
                        {
                            throw new NCException("钻孔点"+item.Key + "未在TVA中出现!");
                          
                        }

                }
            
                display = display + "总安装数量：" + fstqty.ToString() + " TVA:" + tvaqty;
                display = display + "\r\nNC代码仅钻孔数量：\r\n";
                foreach (var item in drillsta)
                {
                    string TVAqty = "";
                    if (alldrilllist.Keys.Contains(item.Key))
                    {
                        TVAqty = alldrilllist[item.Key].ToString();
                        display = display + item.Key.ToString() + "： " + item.Value.ToString() + " TVA:" + alldrilllist[item.Key.ToString()] + "\r\n";

                    }
                    else
                    {
                        throw new NCException("钻孔点"+item.Key + "未在TVA中出现!");

                    }
                   
                }
                

                 display = display + "总钻孔数量：" + drlqty.ToString() + " TVA:" + tvadrillqty;
                fastList = installsta.ToDictionary(a => a.Key, b => b.Value);
                drillList=drillsta.ToDictionary(a => a.Key, b => b.Value);

                return display;

            }
            return "";

          //  label6.Text = display;
          // listBox1.DataSource = this;


        }
        public override string ToString()
        {
            string progstr = "";
            this.ForEach(p => progstr += p);
            return progstr;
        }
        public string BaseRepair(IEnumerable<string> rowprocess)
        {
           
            string showStr = "";
            var fstenerTR = dbinfo.DBfstTable;
            foreach (string kk in rowprocess)
            {
                string tempstr;
                tempstr = kk.Trim();
                tempstr = Regex.Replace(tempstr, @"^N[0-9]*", "", RegexOptions.None);

                if (!tempstr.Contains("(MSG"))
                {
                    tempstr = tempstr.Replace(" ", "");
                }
                tempstr = tempstr.Trim();
                //2015.9.24 Remove M02 for each part
                if (tempstr == "M02")
                {
                    continue;
                }


                //修复换刀T代码bug
                if (tempstr.Contains("M56") && (!tempstr.Contains("T")))
                {
                    tempstr = tempstr.Replace("M56", "M56T");
                }

                //解决M34N/A bug(强制校准bug)
                if (!tempstr.Contains("M34N/A"))
                {
                    tempstr = tempstr.Replace("N/A", "");
                }
                else
                {
                    var ff = fstenerTR.Where(p => p.TCode == System.Convert.ToInt16(FindLast(x => x.Contains("M56")).Replace("M56T", "")));
                    if (ff.Count() == 0)
                    {
                        throw new NCException("代码中出现的紧固件未在TVA中出现");
                    }
                    else
                    {
                        tempstr = "M34" + ff.First().ResyncCode;
                    }


                }




                if (tempstr != "")
                {
                    Add(tempstr);
                    showStr += tempstr + "\r\n";
                }



            }

            return showStr;

        }
        public string ImportFromFile(string filepath,bool ifclear=true)
        {
            if(ifclear)
            {
                this.Clear();
            }
         
            var rowprocess = localMethod.ReadLines(filepath);
           

            return BaseRepair(rowprocess);


        }
        public string ImportFromString(string content, bool ifclear = true)
        {
            if (ifclear)
            {
                this.Clear();
            }
        
            var rowprocess = content.Split(new Char[2] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries).AsEnumerable() ;
            return BaseRepair(rowprocess);

       


        }
        public bool SaveFile(string fileName,bool ifSeq=true)
        {
           
            int indexNo = 0;
            List<string> outputlist=this;

            if (ifSeq)
            {
                outputlist=   this.Select(delegate (string ppp)
                {
                    if (ppp.ElementAt(0) == 'X' || ppp.ElementAt(0) == 'M' || ppp.ElementAt(0) == 'G' || ppp.Contains("MSG"))
                    {
                        indexNo = indexNo + 2;
                        return (   "N" + indexNo.ToString() + " " + ppp);
                       
                    }
                    else
                    {
                        return ppp;
                    }
                }
                ).ToList();
               
               
            }
            if (outputlist.First() != "%")
            {
                outputlist.Insert(0, "%");
            }
            if (outputlist.Last()!= "%")
            {
                outputlist.Insert(outputlist.Count() - 1, "M02");
                outputlist.Insert(outputlist.Count() - 1, "%");
            }


            return  outputlist.WriteFile(fileName);
           
        }


    }
}
