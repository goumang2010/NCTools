using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using FileManagerNew;
using System.Security.Cryptography;
using GoumangToolKit;

namespace NC_TOOL
{
    public partial class ProgramInput : Form
    {
        DBInfo dbinfo;
        NCcodeList unilist;
        public ProgramInput()
        {
            InitializeComponent();
            dbinfo = new DBInfo();
            unilist = new NCcodeList(dbinfo);
            listBox2.DataBindings.Add("DataSource", unilist, "ShowWrongList");
            listBox1.DataBindings.Add("DataSource", unilist, "ShowNCList");

        }



        private void SetWrong(int line,string message)
        {

            unilist.AddToWrongList(message,line);
            
        }

        public List<string> program_part = new List<string>();
        string productnametrim;
        string prodchnname;
 
        //Dictionary<string, int> allfastlist = new Dictionary<string, int>();
        //int  tvaqty = 0;
        bool ifoldprogram = true;

      //  Dictionary<string, int> alldrilllist = new Dictionary<string, int>();
      //  Dictionary<int, string> fstenerT = new Dictionary<int, string>();
        //T代码和校准代码对照
       // Dictionary<int, string> fstenerTR = new Dictionary<int, string>();
      //  Dictionary<string, int> fastlist = new Dictionary<string, int>();
      //  Dictionary<string, int> drilllist = new Dictionary<string, int>();
      //  int tvadrillqty = 0;


        private void program_input_Load(object sender, EventArgs e)
        {





        }
        public string inputValue
        {

            //eg. value=C02312100process
            set
            {

                if (value=="")
                {
                    //传入空字符串，那么就把和产品相关的控件隐藏
                    //屏蔽数据库功能，只进行NC代码检查
                    menuStrip1.Visible = false;
                    button5.Visible = false;
                    comboBox1.Visible = false;
                    checkBox3.Visible = false;
                    label6.Visible = false;
                    label7.Visible = false;
                }
                else
                {
                    menuStrip1.Visible = true;
                    button5.Visible = true;
                    comboBox1.Visible = true;
                    checkBox3.Visible = true;

                   
                }
                LoadTable(value);
               
            }
        }
        /// <summary>
        /// 从数据库中取出产品相关的紧固件数量信息
        /// </summary>
        /// <param name="productName">eg. C02312100process</param>
        private void LoadTable(string productName)
        {
            //如果传入图号不为空，执行页面初始化操作
            if (productName != "")
            {
                //e.g. C02312100
                string prodNameTrim = productName.Replace("process", "");
                //查询程序编号
                //执行sql语句 select 程序编号 from 产品列表 where 图号 like 'C02312100%'
                this.Text = AutorivetDB.QueryNo(prodNameTrim, "程序编号");
                //select FastenerName,count(*) as qty from C02312100 WHERE ProcessType like '%INSTALLED BY%' group by FastenerName order by FastenerName
                dbinfo.DBfastlist = AutorivetDB.TVA_fstlist(prodNameTrim, "INSTALLED BY").AsEnumerable().ToDictionary(k => k[0].ToString(), v => System.Convert.ToInt32(v[1].ToString()));
                dbinfo.DBdrilllist = AutorivetDB.TVA_fstlist(prodNameTrim, "DRILL ONLY BY").AsEnumerable().ToDictionary(k => k[0].ToString(), v => System.Convert.ToInt32(v[1].ToString()));
                //显示TVA中紧固件数量
                string fstdisplay = "TVA紧固件安装数量:\r\n";
                foreach (var pp in dbinfo.DBfastlist)
                {
                    int tempqty = System.Convert.ToInt16(pp.Value.ToString());
                    fstdisplay = fstdisplay + pp.Key + "  ： " + pp.Value.ToString() + "\r\n";
                }
                fstdisplay = fstdisplay + "钻孔的(drill):\r\n";

                foreach (var pp in dbinfo.DBdrilllist)
                {
                    int tempqty = System.Convert.ToInt16(pp.Value.ToString());
                    fstdisplay = fstdisplay + pp.Key + "  ： " + pp.Value + "\r\n";
                }
                label7.Text = fstdisplay;
                productnametrim = productName;
                //获取产品的中文名称
                prodchnname = AutorivetDB.QueryNo(productnametrim, "名称");

                string sql = "SELECT uuid FROM  " + productnametrim + " order by ID";
                program_part = DbHelperSQL.getList(sql);
                comboBox1.DataSource = program_part;

            }
        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ifoldprogram = true;
            textBox1.Text = "";
            if(comboBox1.SelectedIndex!=-1)
            {       
            string pppart = comboBox1.SelectedValue.ToString();
              unilist.ImportFromString( DbHelperSQL.getList("select 程序Program from " + productnametrim + " where UUID='" + pppart + "'").First() );
              CheckAndShow();
            }
            }
        private static bool installpoint(String s)
        {
            if (s.Contains("X") && s.Contains("Y") && s.Contains("Z"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static bool findtcode(String s)
        {
            if (s.Contains("M56"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static bool findgeoset(String s)
        {
            if (s.ToUpper().Contains("START GEOSET"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static bool findop(String s)
        {
            if (s.ToUpper().Contains("START OPERATION"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static bool installpointmm(String s)
        {
            if (s.Contains("M60") || s.Contains("M62"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static bool drillpointmm(String s)
        {
            if (s.Contains("M61") || s.Contains("M63"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private string creatpointinfo(string[] instcoodrow,int pp,List<string> abc)
        {
            string instcood = instcoodrow[0].Trim() + "," + instcoodrow[1].Replace(")", "").Replace("_", ",").Trim();
            //get the geoset
            int geosetindex = abc.FindLastIndex(pp - 1, findgeoset);
            string geosetstr = abc.ElementAt(geosetindex).Split(':')[1];
            geosetstr = geosetstr.Remove(geosetstr.Length - 1);
            geosetstr = geosetstr.Trim();

            //get the operation
            int opindex = abc.FindLastIndex(pp - 1, findop);
            string opstr = abc.ElementAt(opindex).Split(':')[1];
            opstr = opstr.Remove(opstr.Length - 1);
            opstr = opstr.Trim();

            return  instcood + "," + opstr + "," + geosetstr + "," + pp.ToString();
        
        }

        private bool CheckAndShow()
        {
            //ClearWrongList();


            label6.Text = CheckAndShow(unilist, report: checkBox2.Checked, productref: menuStrip1.Visible);

            if (unilist.ShowWrongList.Count==0)
            {
                return true;
            }
            else
            {
              
                return false;
            }
            

        }
        private string CheckAndShow(NCcodeList abc,bool report, bool productref)
        {
            try
            {
                return abc.Check(report: report, productref: productref);
                //unilist.AddRangeToWrongList(abc.ShowWrongList);
              
            }
            catch (NCException ex)
            {
                MessageBox.Show("Check引发异常" + ex.GetType().ToString() + ":" + ex.Message);

            }
            return "";
        }


        private void ClearWrongList()
        {
            unilist.ClearWrongList();

           
        }

        private void button1_Click(object sender, EventArgs e)
        {

            if (textBox1.Text != "")
            {
                unilist.ImportFromString(textBox1.Text);
          
               if( CheckAndShow())
                {
                    MessageBox.Show("已检查完成，无语法问题");
                }
               
                ifoldprogram = false;
                
            }
            else
            {
                MessageBox.Show("文本框里什么也没有，你在逗我？如果只是想重新检查，点击上面的Operation->Re-Trim");
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
           // listBox1.DataSource = null;

             ClearWrongList();
         //   listBox1.Items.Clear();
         
           
           // abc.Clear();
            //abc = new List<string>();
            OpenFileDialog ofd = new OpenFileDialog();
            // ofd.ShowDialog();
            // MessageBox.Show(ofd.FileName);
            //异常检测开始
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = unilist.ImportFromFile(ofd.FileName);
                //关闭此StreamReader对象
            }
           
        }


        private void  ListToText()
        {
            textBox1.Text= unilist.ToString();

        }

        private void button6_Click(object sender, EventArgs e)
        {
            listBox1.TopIndex = listBox1.FindString(listBox1.SelectedItem.ToString());
            listBox1.SelectedIndex = listBox1.TopIndex;
        }



        private void button8_Click(object sender, EventArgs e)
        {
            SaveFileDialog ofd = new SaveFileDialog();
            var templist = new NCcodeList(dbinfo);
           if( ofd.ShowDialog() == DialogResult.OK)
           {

                templist.NCList.AddRange(unilist.NCList);
            if (comboBox1.Visible==true&& checkBox3.Checked==true)
            {
                string firstrow = "(MSG,START PROGRAM PART " + (comboBox1.SelectedIndex + 1).ToString() + " :"+comboBox1.SelectedValue+")";

                    templist.NCList.Insert(1,firstrow);
            }

            if (comboBox1.Visible == true && checkBox3.Checked == true)
            {
                string endrow = "(MSG,END PROGRAM PART " + (comboBox1.SelectedIndex + 1).ToString() + " :" + comboBox1.SelectedValue + ")";
                    templist.NCList.Insert(templist.NCList.Count()-2, endrow);
             }
                templist.SaveFile(ofd.FileName,checkBox1.Checked);
            System.Diagnostics.Process pro = new System.Diagnostics.Process();
            pro.StartInfo.FileName = "notepad.exe";
            
            pro.StartInfo.Arguments = ofd.FileName;
            pro.Start();
               
            MessageBox.Show("生成完毕");

        }
        }

        private void button4_Click(object sender, EventArgs e)
        {

        }

        private void CheckAll(Action<int> proc)
        {
            ClearWrongList();
            comboBox1.SelectedIndex = -1;
           // listBox1.DataSource = null;

            NCcodeList abc = new NCcodeList(dbinfo);
            for (int i = 0; i < comboBox1.Items.Count; i++)
            {
                string pppart = comboBox1.Items[i].ToString();
                proc(i);
            }


        }
        private void test()
        {
            Console.Write("223");
        }
        private void checkAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ClearWrongList();
            comboBox1.SelectedIndex = -1;
         //   listBox1.DataSource = null;
            
            NCcodeList abc = new NCcodeList(dbinfo);

          Action<int> proc = delegate (int i)
               {
                   string pppart = comboBox1.Items[i].ToString();
                   var templist=new NCcodeList(dbinfo);
                   templist.ImportFromString(DbHelperSQL.getList("select 程序Program from " + productnametrim + " where UUID='" + pppart + "'").First());
                 
                   CheckAndShow(templist, false, true);
                  
                   abc.NCList.AddRange(templist.NCList);

               };
           
           CheckAll(proc);

            unilist.NCList = abc.NCList;
             CheckAndShow();
            comboBox1.SelectedIndex = -1;

            var fastcom = from kk in dbinfo.DBfastlist.AsEnumerable()
                     join ll in unilist.fastList.AsEnumerable()
                     on kk.Key equals ll.Key
                     select new
                     {
                         fstName=kk.Key,
                         tvaqty=kk.Value,
                         fstqty=ll.Value

                     };
            var drillcom = from kk in dbinfo.DBdrilllist.AsEnumerable()
                           join ll in unilist.drillList.AsEnumerable()
                           on kk.Key equals ll.Key
                           select new
                           {
                               fstName = kk.Key,
                               tvaqty = kk.Value,
                               drlqty = ll.Value

                           };
            foreach (var pp in fastcom)
            {
                if(pp.fstqty!=pp.tvaqty)
                {
                    string errmsg = pp.fstName + "铆接数量错误！TVA数量：" + pp.tvaqty + "；NC代码数量：" + pp.fstqty;
                    SetWrong(0, errmsg);
                    MessageBox.Show(errmsg);

                }
            }

            foreach (var pp in drillcom)
            {
                if (pp.drlqty!= pp.tvaqty)
                {
                    string errmsg = pp.fstName + "钻孔数量错误！TVA数量：" + pp.tvaqty + "；NC代码数量：" + pp.drlqty;
                    SetWrong(0, errmsg);
                    MessageBox.Show(errmsg);

                }
            }




        }

       private void tiancpara()

       {
            string spqtystr = "";
            Dictionary<string, string> outputlist = new Dictionary<string, string>();
            foreach (var item in unilist.fastList)
            {
                outputlist.Add(item.Key.ToString(), item.Value.ToString());
            }
            foreach (var item in unilist.drillList)
            {
                string fstname = item.Key.ToString();
                if (outputlist.Keys.Contains(fstname))
                {

                    outputlist[fstname] = outputlist[fstname] + "," + item.Value.ToString();
                }
                else
                {
                    outputlist.Add(fstname, item.Value.ToString());
                }
            }
            List<List<string>> paralist = new List<List<string>>();
            string spstr = "";
            if (outputlist.Count() == 1)
            {
                var item = outputlist.First();
                spstr = item.Key.ToString();
                spqtystr = item.Value.ToString();
                paralist.Add(DbHelperSQL.getlistcol("select Process_NO,Countersink_depth,Speed_of_drill,Feed_speed,Clamp_force,Clamp_relief_force,Upset_force,Upset_position,Seal_pres,Seal_time from  紧固件列表 where Fasteners='" + spstr + "'"));

            }
            else
            {
                foreach (var item in outputlist)
                {
                    string fstname = item.Key.ToString();
                    paralist.Add(DbHelperSQL.getlistcol("select Process_NO,Countersink_depth,Speed_of_drill,Feed_speed,Clamp_force,Clamp_relief_force,Upset_force,Upset_position,Seal_pres,Seal_time from  紧固件列表 where Fasteners='" + fstname + "'"));
                    spstr = spstr + "[" + fstname + "]";
                    spqtystr = spqtystr + "[" + item.Value.ToString() + "]";
                }
            }
            

            List<string> resultParaStr=new List<string> ();
            int fstcount = paralist.Count();
            if (fstcount == 0)
            {
                return;
            }
            for (int jj = 0; jj < 10; jj++)
            {
                string tempstr = "";
                bool samepara = true;
                string prestr = paralist[0][jj];
                for (int dd = 0; dd < fstcount; dd++)
                {


                    if (paralist[dd][jj] != prestr)
                    {
                        samepara = false;

                    }
                    prestr = paralist[dd][jj];

                    tempstr = tempstr + "[" + paralist[dd][jj] + "]";



                }

                if (samepara)
                {
                    resultParaStr.Add(prestr);
                }
                else
                {
                    resultParaStr.Add(tempstr);
                }



            }

            DbHelperSQL.ExecuteSql("Update " + productnametrim + " set 紧固件名称Fastener_Name='" + spstr + "',紧固件数量Fastener_Qty='" + spqtystr + "',参数号Process_NO='" + resultParaStr[0] + "',锪窝深度Countersink_depth='" + resultParaStr[1] + "',钻头转速Speed_of_drill='" + resultParaStr[2] + "',给进速率Feed_speed='" + resultParaStr[3] + "',夹紧力Clamp_force='" + resultParaStr[4] + "',夹紧释放力Clamp_relief_force='" + resultParaStr[5] + "',墩铆力Upset_force='" + resultParaStr[6] + "',墩铆位置Upset_position='" + resultParaStr[7] + "',注胶压力Seal_pres='" + resultParaStr[8] + "',注胶时间Seal_time='" + resultParaStr[9] + "' where uuid='" + comboBox1.SelectedValue.ToString() + "'");
        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (ifoldprogram)
            {
                MessageBox.Show("目前是旧程序，请重新打开代码文本，并点击Check；或者点击上面的Operation->Re-Trim来重新检查录入所有的程序段!");
            }
            else
            {

                CheckAndShow();
               var temppro = unilist.ToString();
               // textBox1.Text = temppro;
                tiancpara();
                DbHelperSQL.ExecuteSql("Update " + productnametrim + " set 程序Program='" + temppro + "' where uuid='" + comboBox1.SelectedValue.ToString() + "'");

                MessageBox.Show("执行成功！");
            }


            
        }

        private void outputAllToolStripMenuItem_Click(object sender, EventArgs e)
        {

            //fetch the part quantity

          var partDic=  DbHelperSQL.getDic("select UUID,CONCAT(胶嘴Sealant_Tip,';',下铆头Lower_Anvil) from " + productnametrim );
            int partnum = 0;
            string lastpartname = "";
            //try
            //{
                List<string> tempabc = new List<string>();

              
                string savefolder = localMethod.GetConfigValue("InfoPath") +prodchnname+"_"+ productnametrim.Replace("process","-001") + "\\NC\\";

                if (!System.IO.Directory.Exists(savefolder))
                {
                    System.IO.Directory.CreateDirectory(savefolder);
                }
                //Do not look through sub dictionary
                List<FileInfo> rm = new List<FileInfo>();
                rm.WalkTree(savefolder, false);
                string newfolder = savefolder + "old";
                rm.moveto(newfolder);

                ClearWrongList();
                comboBox1.SelectedIndex = -1;


                ClearWrongList();
                comboBox1.SelectedIndex = -1;
               // listBox1.DataSource = null;

                NCcodeList wholelist = new NCcodeList(dbinfo);

                Action<int> proc = delegate (int i)
                {
                    string pppart = comboBox1.Items[i].ToString();
                    var templist = new NCcodeList(dbinfo);

                    //   templist.Check(true, false, true);
                    List<string> head = new List<string>();
                    head.Add(productnametrim.Replace("process", "").Replace("C0", "O") + (i + 1).ToString());

                    if (partDic.Values.ElementAt(i) != lastpartname)
                    {
                        if (partnum != 0)
                        {
                            head.Add("(Part" + partnum.ToString() + " END)");
                        }

                        partnum = partnum + 1;
                        head.Add("(PART" + partnum.ToString() + " START)");
                        lastpartname = partDic.Values.ElementAt(i);
                    }
                    head.Add("(MSG,START PROGRAM SEGMENT " + (i + 1).ToString() + " :" + pppart + ")");
                    head.Add("(MSG,MAKE SURE THE SEALANT TIP AND LOWER ANVIL:" + partDic[pppart] + ")");
                    templist.ImportFromString(DbHelperSQL.getList("select 程序Program from " + productnametrim + " where UUID='" + pppart + "'").First(),false);
                    if (templist.NCList.Count == 0)
                    {
                        return;
                    }
                    templist.NCList.InsertRange(1, head);
                    
                   
                    templist.NCList.Insert(templist.NCList.Count-2,"(MSG,END PROGRAM SEGMENT " + (comboBox1.SelectedIndex + 1).ToString() + " :" + comboBox1.SelectedValue + ")");
                    string filename = "SEG_" + (i + 1).ToString() + "_" + pppart;
                    string filepath = savefolder + filename;
                    wholelist.NCList.AddRange(templist.NCList);
                   if(!templist.SaveFile(filepath))
                    {
                        throw new NCException("写入文件" + filename + "失败");
                    }

                    
                };
                //Look through all parts and output them
               CheckAll(proc);

                //2015.9.4 change the name to the formal program num.
                string filenameall = this.Text;
                string NCpath = savefolder + filenameall;
                wholelist.SaveFile(NCpath);
                if (!wholelist.NCList.WriteFile(NCpath))
                {
                    throw new NCException("写入文件" + filenameall + "失败");
                }
                DateTime edittime= System.IO.Directory.GetLastWriteTime(savefolder);
                //DateTime nowtime=

               TimeSpan ts1 = new TimeSpan(edittime.Ticks);

               TimeSpan ts2 = new TimeSpan(DateTime.Now.Ticks);

               TimeSpan ts = ts1.Subtract(ts2).Duration();

               System.Diagnostics.Process.Start("explorer.exe", savefolder);
               if( ts.TotalSeconds>5)
               {
                   MessageBox.Show("输出失败");
               }
               else
               {
                    //Update the MD5 and the creation time in database

                    string fileMD5=  localMethod.GetMD5HashFromFile(NCpath);
                    string creationtime = edittime.ToString();
                    Dictionary<string, string> dic = new Dictionary<string, string>();
                    dic.Add("TIME", creationtime);
                    dic.Add("MD5", fileMD5);
                    string jsontext=   localMethod.ToJson(dic);
                  //  string prodchnname = autorivet_op.queryno(productnametrim, "名称");
                    DbHelperSQL.ExecuteSql("update 产品数模 set 备注='"+jsontext+"' where 产品名称='"+prodchnname+ "' and 文件类型='Process'");
                    MessageBox.Show("执行成功！已于"+ creationtime + "更新");
               }

               
            //}
            //catch(Exception ee)
            //{
               
            //    MessageBox.Show("输出失败:"+ee.Message);
            //}



            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            ClearWrongList();
        }

        private void databaseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            setting_database f1 = new setting_database();
            f1.Show();
           
        }

        private void savingFoldersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Save_Folder f1 = new Save_Folder();
            f1.Show();
            
        }

        private void button4_Click_1(object sender, EventArgs e)
        {
           
            ListToText();
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void button9_Click(object sender, EventArgs e)
        {


        }

        private void fillParaToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < comboBox1.Items.Count; i++)
            {
                comboBox1.SelectedIndex = i;
               

                CheckAndShow();

                tiancpara();



            }
            MessageBox.Show("执行成功！");
        }

        private void reTrimToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Action<int> proc = delegate (int i)
            {
                string pppart = comboBox1.Items[i].ToString();
                var templist = new NCcodeList(dbinfo);
                templist.ImportFromString(DbHelperSQL.getList("select 程序Program from " + productnametrim + " where UUID='" + pppart + "'").First());
                CheckAndShow(templist, false, true);
                unilist.AddRangeToWrongList(templist.ShowWrongList);
                DbHelperSQL.ExecuteSql("Update " + productnametrim + " set 程序Program='" + templist.ToString() + "' where uuid='" + pppart + "'");
            };

            CheckAll(proc);
            MessageBox.Show("执行成功！");
        }

        private void listBox2_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            if (listBox2.SelectedIndex != -1)
            {
                string errmsg = listBox2.SelectedItem.ToString();
                    int indexkey =unilist.FetchWrongLineNum(errmsg);
                    if(indexkey>0&&indexkey<listBox1.Items.Count)
                    {
                        listBox1.SelectedIndex = indexkey;
                        listBox1.TopIndex = indexkey;

                    var errarray=   errmsg.Split(',');
                    label3.Text = "Error point info:\r\nProcessFeature编号:\r\n" + errarray[3] + "." + errarray[4] + "\r\nRobotTask:" + errarray[5] + "\r\nOperation:" + errarray[6] + "\r\nGeoset:" + errarray[7] + "\r\n行号：" + errarray[8];


                    }
                   
               

            }
        }
    }
}
