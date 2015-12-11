﻿using MySql.Data.MySqlClient;
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
using mysqlsolution;

namespace NC_TOOL
{
    public partial class program_input : Form
    {
        DBInfo dbinfo;
        NCcodeList unilist;
       
        private void SetWrong(int line,string message)
        {

            unilist.wronglist[message] = line;
            listBox2.DataSource= unilist.wronglist.Keys.ToList();

        }
        public program_input()
        {
            InitializeComponent();
            
           dbinfo = new DBInfo();
           unilist = new NCcodeList(dbinfo);

        }
        public List<string> program_part = new List<string>();
        string productnametrim;
        string prodchnname;
 
        Dictionary<string, int> allfastlist = new Dictionary<string, int>();
        //int  tvaqty = 0;
        bool ifoldprogram = true;

        Dictionary<string, int> alldrilllist = new Dictionary<string, int>();
        Dictionary<int, string> fstenerT = new Dictionary<int, string>();
        //T代码和校准代码对照
        Dictionary<int, string> fstenerTR = new Dictionary<int, string>();
        Dictionary<string, int> fastlist = new Dictionary<string, int>();
        Dictionary<string, int> drilllist = new Dictionary<string, int>();
        int tvadrillqty = 0;


        private void program_input_Load(object sender, EventArgs e)
        {



        }
        public string inputValue
        {
           

            set
            {

                if (value=="")
                {
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
                loadtable(value);
               
            }
        }

        private void loadtable(string productname)
        {
            if(productname!="")
            {
                string prodnametrim = productname.Replace("process", "");
                this.Text = AutorivetDB.queryno(prodnametrim, "程序编号");

                dbinfo.DBfastlist=AutorivetDB.TVA_fstlist(prodnametrim, "INSTALLED BY").AsEnumerable().ToDictionary(k=>k[0].ToString(),v=>System.Convert.ToInt32( v[1].ToString()));
                dbinfo.DBdrilllist= AutorivetDB.TVA_fstlist(prodnametrim, "DRILL ONLY BY").AsEnumerable().ToDictionary(k => k[0].ToString(), v => System.Convert.ToInt32(v[1].ToString()));



                //  DbHelperSQL.Query("select FastenerName,count(*) as qty from " + productname.Replace("process", "") + " WHERE ProcessType like '%DRILL ONLY BY%' group by FastenerName order by FastenerName").Tables[0];
                string fstdisplay = "TVA紧固件安装数量:\r\n";
            foreach(var pp in dbinfo.DBfastlist)
            {
                int tempqty=System.Convert.ToInt16(pp.Value.ToString());
                fstdisplay = fstdisplay +pp.Key+ "  ： " + pp.Value.ToString() + "\r\n";
                allfastlist.Add(pp.Key.ToString(),tempqty);
               
              
            }
            fstdisplay = fstdisplay + "钻孔的(drill):\r\n";

            foreach (var pp in dbinfo.DBdrilllist)
            {
                int tempqty = System.Convert.ToInt16(pp.Value.ToString());
                fstdisplay = fstdisplay + pp.Key + "  ： " + pp.Value + "\r\n";
                alldrilllist.Add(pp.Key.ToString(), tempqty);

                tvadrillqty = tvadrillqty + tempqty;
                // tvaqty=
            }



            label7.Text = fstdisplay;

            try
            {

                productnametrim = productname;
                  prodchnname = AutorivetDB.queryno(productnametrim, "名称");

                    MySqlConnection MySqlConn = new MySqlConnection(PubConstant.ConnectionString);
                MySqlConn.Open();
                String sql = "SELECT uuid FROM  " + productnametrim +" order by ID";
                program_part = DbHelperSQL.getlist(sql);


                comboBox1.DataSource = program_part;


                //   this.dataGridView1.DataSource = dt;

            }
            catch
            {
                MessageBox.Show("当前数据库不可用，请更换数据库");

            }

            }

            else
            {
                DataTable fsttemp = DbHelperSQL.Query("select Fasteners from 紧固件列表").Tables[0];
                foreach (DataRow pp in fsttemp.Rows)
                {
                    allfastlist.Add(pp[0].ToString(), 0);
                    alldrilllist.Add(pp[0].ToString(), 0);


                }

            }


        }
        private void textBox1_TextChanged(object sender, EventArgs e)
        {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            ifoldprogram = true;

            if(comboBox1.SelectedIndex!=-1)
            {       
            string pppart = comboBox1.SelectedValue.ToString();
              unilist.ImportFromString( DbHelperSQL.getlist("select 程序Program from " + productnametrim + " where UUID='" + pppart + "'").First() );
               checkdupi();
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

        private bool checkdupi()
        {
            clearall();

            try
            {
                label6.Text=    unilist.Check(report: checkBox2.Checked, productref: menuStrip1.Visible);
            }
            catch (Exception ex)
            {
                MessageBox.Show("unilist.Check引发异常"+ex.GetType().ToString() + ":" + ex.Message);

            }
           
          
            listBox1.DataSource = unilist;
            if(unilist.wronglist.Count==0)
            {
                return true;
            }
            else
            {
                listBox2.DataSource = unilist.wronglist.Keys.ToList();
                return false;
            }
            

        }

        private List<string> creatlist(string text)
        {
            string[] rowprocess;
            List<string> abc = new List<string>();


            rowprocess = text.Split(new Char[2] { '\r', '\n' }, System.StringSplitOptions.RemoveEmptyEntries);

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
                    tempstr = "M34" + fstenerTR[System.Convert.ToInt16(abc.FindLast(findtcode).Replace("M56T", ""))];

                }

           


                if (tempstr != "")
                {
                    abc.Add(tempstr);
                }

            }
            return abc;
        }
        private void clearall()
        {

            
            listBox2.DataSource = null;
        }

        private void button1_Click(object sender, EventArgs e)
        {

            if (textBox1.Text != "")
            {
                unilist.ImportFromString(textBox1.Text);
          
               if( checkdupi())
                {
                    MessageBox.Show("已检查完成，无语法问题");
                }
               
                ifoldprogram = false;
                
            }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            listBox1.DataSource = null;

            unilist.wronglist.Clear();
            listBox1.Items.Clear();
           // listBox2.Items.Clear();

           
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

        private void listBox2_SelectedIndexChanged(object sender, EventArgs e)
        {

            
        }
        private string  listtotext(List<string> abc)
        {
            
            string newtext = "";

            foreach( string ddd in abc)
            {
                newtext = newtext + ddd + "\r\n";
            }
            textBox1.Text = newtext;
            return newtext;
        }

        private void button6_Click(object sender, EventArgs e)
        {
            listBox1.TopIndex = listBox1.FindString(listBox1.SelectedItem.ToString());
            listBox1.SelectedIndex = listBox1.TopIndex;
        }

        private void button7_Click(object sender, EventArgs e)
        {
            List<string> abc = (List<string>)listBox1.DataSource;
            abc.RemoveAt(listBox1.SelectedIndex);
            listBox1.DataSource = null;
            listBox1.DataSource = abc;
            listtotext(abc);
            listBox2.Items.Clear();
            //textBox1.Text=listBox1.Items.
        }

        private void button8_Click(object sender, EventArgs e)
        {
            SaveFileDialog ofd = new SaveFileDialog();
           
           if( ofd.ShowDialog() == DialogResult.OK)
           {


            if (comboBox1.Visible==true&& checkBox3.Checked==true)
            {
                string firstrow = "(MSG,START PROGRAM PART " + (comboBox1.SelectedIndex + 1).ToString() + " :"+comboBox1.SelectedValue+")";
                
                unilist.Insert(0,firstrow);
            }

            if (comboBox1.Visible == true && checkBox3.Checked == true)
            {
                string endrow = "(MSG,END PROGRAM PART " + (comboBox1.SelectedIndex + 1).ToString() + " :" + comboBox1.SelectedValue + ")";
                    unilist.Insert(unilist.Count()-1, endrow);
             }
                unilist.SaveFile(ofd.FileName,checkBox1.Checked);
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
            clearall();
            comboBox1.SelectedIndex = -1;
            listBox1.DataSource = null;

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
            clearall();
            comboBox1.SelectedIndex = -1;
            listBox1.DataSource = null;
            
            NCcodeList abc = new NCcodeList(dbinfo);

          Action<int> proc = delegate (int i)
               {
                   string pppart = comboBox1.Items[i].ToString();
                   var templist=new NCcodeList(dbinfo);
                   templist.ImportFromString(DbHelperSQL.getlist("select 程序Program from " + productnametrim + " where UUID='" + pppart + "'").First());
                   templist.Check(true, false, true);
                   abc.AddRange(templist);

               };
           
           CheckAll(proc);

            unilist = abc;
             checkdupi();
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
            string jingujian = "";

                string jingujianshuliang = "";

                Dictionary<string, string> outputlist = new Dictionary<string, string>();






                foreach (var item in fastlist)
                {
                    outputlist.Add(item.Key.ToString(), item.Value.ToString());

                   // jingujian = "["+jingujian + item.Key.ToString() + "： " + item.Value.ToString() + " TVA:" + allfastlist[item.Key.ToString()] + "\r\n";


                }
              
                foreach (var item in drilllist)
                {
                    string fstname=item.Key.ToString();
                    if (outputlist.Keys.Contains(fstname))
                    {
                        string oldqty= outputlist[fstname];
                        outputlist[fstname] = oldqty + "," + item.Value.ToString() ;
                    }
                    else
                    {
                        outputlist.Add(fstname, item.Value.ToString() );
                    }


                  //  display = display + item.Key.ToString() + "： " + item.Value.ToString() + " TVA:" + alldrilllist[item.Key.ToString()] + "\r\n";

                }
                List<List<string>> canshu = new List<List<string>>();

                foreach (var item in outputlist)
                {
                    string fstname = item.Key.ToString();
                   List<string> templist=(DbHelperSQL.getlistcol("select Process_NO,Countersink_depth,Speed_of_drill,Feed_speed,Clamp_force,Clamp_relief_force,Upset_force,Upset_position,Seal_pres,Seal_time from  紧固件列表 where Fasteners='" + fstname + "'"));

                   canshu.Add(templist);
                   if (outputlist.Count()==1)
                   {
                       jingujian = fstname;
                       jingujianshuliang = item.Value.ToString();
                   }
                   else
                   {
                       jingujian = jingujian + "[" + fstname + "]";

                       jingujianshuliang = jingujianshuliang + "[" + item.Value.ToString() + "]";
                       // jingujian = "["+jingujian + item.Key.ToString() + "： " + item.Value.ToString() + " TVA:" + allfastlist[item.Key.ToString()] + "\r\n";

                   }
                   // canshu.Add
                  
                }
                List<string> resultcanshu=new List<string> ();
                int fstcount=canshu.Count();
            if (fstcount==0)
            {
                return;
            }
                for (int jj=0;jj<10;jj++)
                    {
                        string tempstr="";
                    bool samepara=true;
                    string prestr = canshu[0][jj];
                      for(int dd=0;dd<fstcount;dd++)
                {


                      if(canshu[dd][jj]!=prestr) 
                      {
                          samepara=false;

                      }
                          prestr=canshu[dd][jj];

                  tempstr=tempstr+"[" + canshu[dd][jj]+"]";



                    }

                    if(samepara)
                    {
                        resultcanshu.Add(prestr);
                    }
                    else
                    {
                         resultcanshu.Add(tempstr);
                    }
                   


                }
                
                DbHelperSQL.ExecuteSql("Update " + productnametrim + " set 紧固件名称Fastener_Name='" + jingujian + "',紧固件数量Fastener_Qty='"+ jingujianshuliang+"',参数号Process_NO='"+resultcanshu[0]+"',锪窝深度Countersink_depth='"+resultcanshu[1]+"',钻头转速Speed_of_drill='"+resultcanshu[2]+"',给进速率Feed_speed='"+resultcanshu[3]+"',夹紧力Clamp_force='"+resultcanshu[4]+"',夹紧释放力Clamp_relief_force='"+resultcanshu[5]+"',墩铆力Upset_force='"+resultcanshu[6]+"',墩铆位置Upset_position='"+resultcanshu[7]+"',注胶压力Seal_pres='"+resultcanshu[8]+"',注胶时间Seal_time='"+resultcanshu[9]+"' where uuid='" + comboBox1.SelectedValue.ToString() + "'");
         

           
       }

        private void button5_Click(object sender, EventArgs e)
        {
            if (ifoldprogram)
            {
                MessageBox.Show("目前是旧程序，请重新点击Check");
            }
            else
            {

                checkdupi();
               var temppro = unilist.ToString();
                textBox1.Text = temppro;
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
            try
            {
                List<string> tempabc = new List<string>();

              
                string savefolder = localMethod.InfoPath +prodchnname+"_"+ productnametrim.Replace("process","-001") + "\\NC\\";

                if (!System.IO.Directory.Exists(savefolder))
                {
                    System.IO.Directory.CreateDirectory(savefolder);
                }
                //Do not look through sub dictionary
                List<FileInfo> rm = new List<FileInfo>();
                rm.WalkTree(savefolder, false);
                string newfolder = savefolder + "old";
                rm.moveto(newfolder);

                clearall();
                comboBox1.SelectedIndex = -1;


                clearall();
                comboBox1.SelectedIndex = -1;
                listBox1.DataSource = null;

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
                    templist.ImportFromString(DbHelperSQL.getlist("select 程序Program from " + productnametrim + " where UUID='" + pppart + "'").First(),false);
                    templist.InsertRange(1, head);
                    templist.Insert(templist.Count-2,"(MSG,END PROGRAM SEGMENT " + (comboBox1.SelectedIndex + 1).ToString() + " :" + comboBox1.SelectedValue + ")");
                    string filename = "SEG_" + (i + 1).ToString() + "_" + pppart;
                    string filepath = savefolder + filename;
                    wholelist.AddRange(templist);
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
                if (!wholelist.WriteFile(NCpath))
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

               
            }
            catch(Exception ee)
            {
               
                MessageBox.Show("输出失败:"+ee.Message);
            }



            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            clearall();
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
            List<string> abc = (List<string>)listBox1.DataSource;
            listtotext(abc);
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
               

                checkdupi();

                tiancpara();



            }
            MessageBox.Show("执行成功！");
        }

        private void reTrimToolStripMenuItem_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < comboBox1.Items.Count; i++)
            {
                comboBox1.SelectedIndex = i;
                string value = comboBox1.SelectedValue.ToString();
                List<string> abc1 = (List<string>)listBox1.DataSource;
                
            string newprogram=  listtotext (creatlist(listtotext(abc1)));

            DbHelperSQL.ExecuteSql("Update " + productnametrim + " set 程序Program='" + newprogram + "' where uuid='" + comboBox1.SelectedValue.ToString() + "'");



            }
            MessageBox.Show("执行成功！");
        }

        private void listBox2_SelectedIndexChanged_1(object sender, EventArgs e)
        {
            if (listBox2.SelectedIndex != -1)
            {
                try
                {
                    int indexkey = unilist.wronglist[listBox2.SelectedItem.ToString()];
                    listBox1.SelectedIndex = indexkey;
                    listBox1.TopIndex = indexkey;
                }
                catch
                {

                }

            }
        }
    }
}