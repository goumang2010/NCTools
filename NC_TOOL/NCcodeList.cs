using GoumangToolKit;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;


namespace NC_TOOL
{
    public  class NCcodeList : INotifyPropertyChanged
    {
        private IDBInfo dbinfo;
        private List<string> codeList = new List<string>();
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }

        }
        //property
        //the installed fasterners this NC codes covers
        public Dictionary<string, int> fastList { get; set; }
        public Dictionary<string, int> drillList { get; set; }

        #region InputData

        public NCcodeList(IDBInfo dbInfo)
        {
            dbinfo = dbInfo;
        }
        public string ImportFromFile(string filepath, bool ifclear = true)
        {
            if (ifclear)
            {
                codeList.Clear();
            }

            var rowprocess = FileIO.ReadLines(filepath);


            return BaseRepair(rowprocess);


        }
        public string ImportFromString(string content, bool ifclear = true)
        {
            if (ifclear)
            {
                codeList.Clear();
            }

            var rowprocess = content.Split(new Char[2] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries).AsEnumerable();
            return BaseRepair(rowprocess);




        }
        #endregion
        #region OutputData

        public List<string> NCList
        {
            get
            {

                return codeList;
            }
            set
            {
                codeList = value;
            }


        }
        public List<string> ShowNCList
        {
            //Only for data binding
            get
            {
                //For biond the list box,every time generate a new list
                return codeList.ToList();
            }


        }

        public override string ToString()
        {
            string progstr = "";
            codeList.ForEach(p => progstr += p + "\r\n");
            var count = progstr.Count();
            if (count >= 2)
            {
                progstr = progstr.Remove(count - 2);
            }
            
            return progstr;
        }


        public bool SaveFile(string fileName, bool ifSeq = true)
        {

            int indexNo = 0;
            List<string> outputlist = codeList;
            if(outputlist.Count==0)
            {
                return false;
            }

                if (ifSeq)
            {
                outputlist = codeList.Select(delegate (string ppp)
                {
                    if (ppp.ElementAt(0) == 'X' || ppp.ElementAt(0) == 'M' || ppp.ElementAt(0) == 'G')
                    {
                        indexNo = indexNo + 2;
                        return ("N" + indexNo.ToString() + " " + ppp);

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
            if (outputlist.Last() != "%")
            {
                outputlist.Insert(outputlist.Count() - 1, "M02");
                outputlist.Insert(outputlist.Count() - 1, "%");
            }


            return outputlist.WriteFile(fileName);

        }
        #endregion
        #region RepairAndCheck
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
                    var ff = fstenerTR.Where(p => p.TCode == System.Convert.ToInt16(codeList.FindLast(x => x.Contains("M56")).Replace("M56T", "")));
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
                    codeList.Add(tempstr);
                    showStr += tempstr + "\r\n";
                }



            }

            // OnPropertyChanged("ShowNCList");
            ClearWrongList();
            return showStr;

        }

        private NCpointCoord CreatPointInfo(string[] instcoodrow, int pp)
        {
            //Get the task
           var taskrow=  codeList.FindIndex(p => p == "(PN=)") -1;
            string taskname = "";
            if(taskrow>=0)
            {
                taskname = codeList[taskrow];
            }
       
            var tncount = taskname.Count();
            if (tncount > 2)
            {
                taskname = taskname.Substring(1, tncount - 2);
            }
           


            //get the geoset
            int geosetindex = codeList.FindLastIndex(pp - 1,s=> s.ToUpper().Contains("START GEOSET"));
            string geosetstr = codeList.ElementAt(geosetindex).Split(':')[1];
            geosetstr = geosetstr.Remove(geosetstr.Length - 1);
            geosetstr = geosetstr.Trim();

            //get the operation
            int opindex = codeList.FindLastIndex(pp - 1, s=> s.ToUpper().Contains("START OPERATION"));
            string opstr = codeList.ElementAt(opindex).Split(':')[1];
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
                PFNum = System.Convert.ToInt32(pfname[1]),
                Geoset = geosetstr,
                Operation = opstr,
                RowNum= pp,
                RobotTask=taskname
                

            };
            return pt;

        }

      
      public List<NCpointCoord> CheckPoints (Predicate<string> Moperation)
        {

            var fstenerT = dbinfo.DBfstTable;

            int McodeRow = codeList.FindIndex(0, Moperation);
            int CoordRow = 0;
            var installlistfull = new List<NCpointCoord>();
            int uid = 0;

            while (McodeRow > 0)
            {
                CoordRow = codeList.FindLastIndex(McodeRow - 1, s => (s.Contains("X") && s.Contains("Y") && s.Contains("Z")));
                //Split the coord and pf name
                string[] instcoodrow = codeList.ElementAt(CoordRow).Split('(');
                //Create coord infomation
                var instcood = CreatPointInfo(instcoodrow, CoordRow);

                string Tcode;
                int TcodeRow = codeList.FindLastIndex(McodeRow - 1, s => s.Contains("M56"));
                if (TcodeRow < 0)
                {
                    throw new NCException("这不是完整的程序，没有包含选钉代码");
                }
                else
                {
                    Tcode = codeList[TcodeRow].Replace("M56T", "");
                }
                //Get the fasterner name per dictionary
                string fstnameT = fstenerT.getFastName(System.Convert.ToInt16(Tcode));
                if (fstnameT != instcood.PFName)
                {
                    wronglist["ProcessFeature名称错误,SX_CENIT," + instcood.OutPut()] = CoordRow;
                    // throw new NCException(shuchu);
                }
                instcood.PFName = fstnameT;
                //Remove the number after the dot,make the coord integer
                string instcoodsimple2 = instcood.UUID;
             var lastdupliptIndex=  installlistfull.FindIndex(p => p.UUID == instcood.UUID);
               
                if (lastdupliptIndex != -1 )
                {
                    var lastduplipt = installlistfull[lastdupliptIndex];
                    //If the duplicate is a neighbored one,then judge it by A axis, they are only different on A Axis,then they may be ring points not the duplicates
                    if (installlistfull.IndexOf(lastduplipt) == installlistfull.Count - 1)
                    {
                        double mm1 = System.Convert.ToDouble(lastduplipt.A);
                        double mm2 = System.Convert.ToDouble(instcood.A);
                        if (Math.Abs(mm1 - mm2) < 0.4)
                        {

                            wronglist["A坐标相近," + uid + "," + lastduplipt.OutPut()]= lastduplipt.RowNum;
                            wronglist["A坐标相近," + uid + "," + instcood.OutPut()] = CoordRow;
                        }
                        //如果不相近，则正常记录
                       
                           installlistfull.Add(instcood);
                        

                    }
                    else
                    {
                        wronglist["重复点," + uid + "," + lastduplipt.OutPut()] = lastduplipt.RowNum;
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
                McodeRow = codeList.FindIndex(McodeRow + 1, Moperation);

            }
            
            //OnPropertyChanged("ShowWrongList");

            return installlistfull;
        }
      public string Check(bool erroroutput = true,bool report=false,bool productref=true)
        {

            ClearWrongList();
           
            var installlistfull = CheckPoints(x => x.Contains("M60") || x.Contains("M62"));
            var drilllistfull = CheckPoints(x => x.Contains("M61") || x.Contains("M63"));

            //合并检查钻孔及铆接的重复点
            var crosscheck = from aa in installlistfull
                             join bb in drilllistfull
                             on aa.UUID equals bb.UUID
                             select new
                             {
                                 install = aa,
                                 drill = bb
                             };
            int uid = 100;
            foreach(var vv in crosscheck)
            {
                wronglist["重复点," + uid + "," + vv.install.OutPut()] = vv.install.RowNum;
                wronglist["重复点," + uid + "," + vv.drill.OutPut()] = vv.drill.RowNum;
            }
            OnPropertyChanged("ShowWrongList");
            OnPropertyChanged("ShowNCList");
            if (report)
            {
             
                DataTable dt = new DataTable();
                dt.Columns.Add("坐标Coord", typeof(string));
                dt.Columns.Add("紧固件Fastener", typeof(string));
                dt.Columns.Add("PF Index", typeof(string));
                dt.Columns.Add("RobotTask Name", typeof(string));
                dt.Columns.Add("Operation Name", typeof(string));
                dt.Columns.Add("Geoset Name", typeof(string));
                dt.Columns.Add("程序行号Program Row", typeof(string));

               
                foreach (var item in installlistfull)
                {
                    dt.Rows.Add(item.ToArray());
                }
                if (dt.Rows.Count > 0)
                {
                    OfficeMethod.excelMethod.SaveDataTableToExcel(dt);
                }
            }
            if (erroroutput)
            {
                DataTable dt = new DataTable();
                dt.Columns.Add("错误类型", typeof(string));
                dt.Columns.Add("UID", typeof(string));
                dt.Columns.Add("坐标Coord", typeof(string));
                dt.Columns.Add("紧固件Fastener", typeof(string));
                dt.Columns.Add("PF Index", typeof(string));
                dt.Columns.Add("RobotTask Name", typeof(string));
                dt.Columns.Add("Operation Name", typeof(string));
                dt.Columns.Add("Geoset Name", typeof(string));
                dt.Columns.Add("程序行号Program Row", typeof(string));

                foreach (string item in wronglist.Keys)
                {
                    dt.Rows.Add(item.Split(','));
                }
               var bb= dt.Select("错误类型<>'ProcessFeature名称错误'").Count();
                if (dt.Rows.Count > 0&&bb>0)
                {
                    OfficeMethod.excelMethod.SaveDataTableToExcel(dt);
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

        #endregion
        #region WrongList
        private Dictionary<string, int> wronglist = new Dictionary<string, int>();

        public List<string> ShowWrongList
        {
            get
            {
                return wronglist.Keys.ToList();
            }
        }
        public void AddToWrongList(string msg, int i)
        {
            wronglist[msg] = i;
            OnPropertyChanged("ShowWrongList");
        }
        public void AddRangeToWrongList(List<string> otherErrList)
        {
            otherErrList.ForEach(p => AddToWrongList(p, 0));
            OnPropertyChanged("ShowWrongList");
        }
        public int FetchWrongLineNum(string info)
        {
            if (wronglist.ContainsKey(info))
            {
                return wronglist[info];

            }
            return -1;

        }
        public void ClearWrongList()
        {
            wronglist.Clear();
            OnPropertyChanged("ShowWrongList");
        }

        #endregion



    }
}
