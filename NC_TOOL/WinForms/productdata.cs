using MySql.Data.MySqlClient;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.OleDb;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using mysqlsolution;



namespace NC_TOOL
{
    public partial class ProductData : Form
    {

        private DataTable dt = new DataTable();
  //  private OleDbDataAdapter daOleDb = new OleDbDataAdapter();
    private MySqlDataAdapter daMySql = new MySqlDataAdapter();
        
       string productnametrim ;
       string prodname;
        public ProductData()
        {
            InitializeComponent();
        }


        public string inputValue
        {
            get
            {
                return this.textBox1.Text;
            }
            set
            {

                this.textBox1.Text = value;
                loadtable(value);
            }
        }
        private void  loadtable(string productname)
        {
            try
            {
                

                //传递的是带process的
                productnametrim = productname;
             
                //产品图号

              prodname = productnametrim.Replace("process", "-001");

                MySqlConnection MySqlConn = new MySqlConnection(PubConstant.ConnectionString);
                    MySqlConn.Open();
                    String sql = "SELECT * FROM  " + productnametrim + " order by ID";
                    daMySql = new MySqlDataAdapter(sql, MySqlConn);
                   // DataSet OleDsyuangong = new DataSet();
                    dt = new DataTable();
                    daMySql.Fill(dt);

                    this.dataGridView1.DataSource = dt;

                    dataGridView2.DataSource = DbHelperSQL.Query("select concat('T',编号) as 组合编号,concat('C1-SKIN-',蒙皮厚度) as 试片1,concat('C1-',二层材料,'-',二层厚度) as 试片2,count(*) as 数量 from 试片列表 where 产品图号='" + prodname + "' group by 编号 order by 编号").Tables[0];
                  //  dataGridView2.Columns[0].Width = 40;
                  //  dataGridView2.Columns[0].Width = 40;
                 //  dataGridView2.Columns[0].Width = 40;

                 //   this.dataGridView1.DataSource = dt;
              
            }
            catch
            {
                MessageBox.Show("当前数据库不可用，请更换数据库");

            }
        }
        private void Form1_Load(object sender, EventArgs e)
        {
         //   dataGridView2.DataSource = DbHelperSQL.Query("select concat('T',编号) as 组合编号,concat('C1-SKIN-',蒙皮厚度) as 试片1,concat('C1-',二层材料,'-',二层厚度) as 试片2,count(*) as 数量 from 试片列表 where 产品图号='" + prodname + "' group by 编号 order by 编号").Tables[0];
            
        }



        private void getupdate( DataTable dt)
        {
          //dt = dataGridView1.DataSource as DataTable;//把DataGridView绑定的数据源转换成DataTable 
  
                MySqlCommandBuilder cb = new MySqlCommandBuilder(daMySql);

                daMySql.Update(dt);

        }




        private void button3_Click(object sender, EventArgs e)
        {
            int row = dataGridView1.SelectedRows[0].Index;
            DataRow insertrow = ((DataRowView)dataGridView1.SelectedRows[0].DataBoundItem).Row;

           // int row = dataGridView1.SelectedRows.Count;
          //  if (MessageBox.Show("确认删除第" + row.ToString() + "条记录吗？", "请确认", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
         //   {
               try
               {
                   string delcoupon = "delete from 试片列表 where 产品图号='" + prodname + "' and 程序段编号='" + insertrow[1].ToString() + "';";
                   DbHelperSQL.ExecuteSql(delcoupon);
                    ((DataRowView)dataGridView1.SelectedRows[0].DataBoundItem).Row.Delete();
                  //  row = row - 1;

                    for (int k = row ; k < dt.Rows.Count-1; k++)
                    {
                        DataRow lastrow = ((DataRowView)dataGridView1.Rows[k].DataBoundItem).Row;
                        lastrow[0] = System.Convert.ToInt16(lastrow[0]) -1;
                    }
                    }
            catch
               {

                   MessageBox.Show("删除时发生错误，请注意编号");
               }
            finally
               {
                   this.getupdate(dt);
               }

          /*
            if (row>1)
            {
                dataGridView1.Rows[row - 1].Selected = true;
            }
            else
            {
                dataGridView1.Rows[row + 1].Selected = true;
            }
           * */
               try
               {
                   dataGridView1.Rows[row].Selected = true;
               }
         catch
               {
                   MessageBox.Show("删除时发生错误，请注意编号");


               }
               loadtable(productnametrim);
               // MessageBox.Show("删除成功");
          //  }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.getupdate(dt);
            MessageBox.Show("更新成功");
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            viewExcel();
        }
        public void viewExcel()
        {
        var tianchong = new Dictionary<string, DataTable>();

         // List<string> tianchongname=new List<string> ();


          tianchong.Add(productnametrim,AutorivetDB.getparatable(productnametrim));
            //  tianchongname.Add(productnametrim);

            OFFICE_Method.excelMethod.SaveDataTableToExcelTran(tianchong);

          //  OFFICE_Method. excelMethod.SaveDataTableToExcel(tianchong);
          //   MsgBox("执行成功")
        }
  
        
        public void newRow(DataRow newRow)
        {
    
                dt.Rows.Add(newRow);
                
           
        }





        private void button2_Click_1(object sender, EventArgs e)
        {
            this.getupdate(dt);
            MessageBox.Show("更新成功");
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
           
           var f1 = new program_input();
            f1.inputValue = textBox1.Text;
            f1.Show();
            this.Close();


        }



        private void button5_Click(object sender, EventArgs e)
        {
          //  dt = dataGridView1.DataSource as DataTable;
            renum();
            int row = dataGridView1.SelectedRows[0].Index;

            //DataRow temprow = dt.NewRow();


                  //  ((DataRowView)dataGridView1.SelectedRows[0].DataBoundItem).Row.Delete();
            DataRow insertrow = ((DataRowView)dataGridView1.SelectedRows[0].DataBoundItem).Row;
            DataRow lastrow = ((DataRowView)dataGridView1.Rows[row-1].DataBoundItem).Row;
        //    DataTable dtt = (DataRowView)dataGridView1.data.DataBindings.;

                  //  temprow[0] = lastrow[0];

           
                 int tempid = (int)insertrow[0];
                 insertrow[0] = lastrow[0];
                 lastrow[0] = tempid;

                 this.getupdate(dt);
         
               loadtable(productnametrim);
              //      dt.Rows.InsertAt(temprow,   newrow);
                   // temprow.Delete();
              //      this.getupdate(dt);
                  
              //      dataGridView1.Rows[ newrow].Selected = true;
               dataGridView1.Rows[row-1].Selected = true;
                    //this.getupdate(dt);
                
            //    this.getupdate(dt);
             //   MessageBox.Show("成功");
            
        }

        private int renum()
    {
        int count = dt.Rows.Count;
        for (int i = 0; i < count; i++)
        {
            dt.Rows[i][0]= i + 1;
        }
            //返回最大编号
        return count;
            

      //  this.getupdate(dt);

      //  loadtable(productnametrim);
    }



        private void button6_Click(object sender, EventArgs e)
        {
            //  dt = dataGridView1.DataSource as DataTable;
            int row = dataGridView1.SelectedRows[0].Index;

            //DataRow temprow = dt.NewRow();


            //  ((DataRowView)dataGridView1.SelectedRows[0].DataBoundItem).Row.Delete();
            DataRow insertrow = ((DataRowView)dataGridView1.SelectedRows[0].DataBoundItem).Row;
            DataRow lastrow = ((DataRowView)dataGridView1.Rows[row + 1].DataBoundItem).Row;
            //    DataTable dtt = (DataRowView)dataGridView1.data.DataBindings.;

            //  temprow[0] = lastrow[0];


            int tempid = (int)insertrow[0];
            insertrow[0] = lastrow[0];
            lastrow[0] = tempid;

            this.getupdate(dt);

            loadtable(productnametrim);
            //      dt.Rows.InsertAt(temprow,   newrow);
            // temprow.Delete();
            //      this.getupdate(dt);

            //      dataGridView1.Rows[ newrow].Selected = true;
            dataGridView1.Rows[row + 1].Selected = true;
            //this.getupdate(dt);

            //    this.getupdate(dt);
            //   MessageBox.Show("成功");
        }

        private void button7_Click(object sender, EventArgs e)
        {
      
            int row = dataGridView1.SelectedRows[0].Index;
            DataRow insertrow = ((DataRowView)dataGridView1.SelectedRows[0].DataBoundItem).Row;
            string sqlstr = "Insert into " + productnametrim + " (ID,UUID,加工位置location,紧固件名称Fastener_Name,紧固件数量Fastener_Qty,钻头Drill,下铆头Lower_Anvil,上铆头Upper_Anvil,胶嘴Sealant_Tip) values (";
            sqlstr = sqlstr + (row+2).ToString()+",";
            sqlstr = sqlstr +"'"+ insertrow[1].ToString() + "_2',";

            for(int k=row + 1;k<dt.Rows.Count;k++)
            {
                DataRow lastrow = ((DataRowView)dataGridView1.Rows[k].DataBoundItem).Row;
                lastrow[0] = System.Convert.ToInt16(lastrow[0]) + 1;
            }


            this.getupdate(dt);

          //  for(int i=2;i<4;i++)
          //  {
          //      sqlstr = sqlstr +"'"+ insertrow[i].ToString() + "',";
          //  }

          //  sqlstr = sqlstr +   insertrow[4].ToString() + ",";
            for (int i = 2; i < 9; i++)
            {
                sqlstr = sqlstr + "'" + insertrow[i].ToString() + "',";
            }
            sqlstr=sqlstr.Remove(sqlstr.Count() - 1);
            sqlstr = sqlstr + ")";
            DbHelperSQL.ExecuteSql(sqlstr);
            loadtable(productnametrim);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            filother();
            
            
         //  this.getupdate(dt);
            loadtable(productnametrim);
        }

        private void filother()
    {
        List<string> exesql = new List<string>();
          
        string sqlstr = "";
            string sqlstr1 = "";
            if (checkBox1.Checked == true)
        {

            sqlstr1 = "Update " + productnametrim + " pp inner join 紧固件列表 ll on ll.Fasteners like concat(pp.紧固件名称Fastener_Name,'%') set pp.参数号Process_NO=ll.Process_NO,pp.锪窝深度Countersink_depth=ll.Countersink_depth,pp.钻头转速Speed_of_drill=ll.Speed_of_drill,pp.给进速率Feed_speed=ll.Feed_speed,pp.夹紧力Clamp_force=ll.Clamp_force,pp.夹紧释放力Clamp_relief_force=ll.Clamp_relief_force,pp.墩铆力Upset_force=ll.Upset_force,pp.墩铆位置Upset_position=ll.Upset_position,pp.注胶压力Seal_pres=ll.Seal_pres,pp.注胶时间Seal_time=ll.Seal_time,pp.钻头Drill=ll.Drill,pp.上铆头Upper_Anvil=ll.Upper_Anvil,pp.胶嘴Sealant_Tip=ll.Tips";
                sqlstr = "Update " + productnametrim + " pp left join 紧固件列表 ll on pp.UUID like concat('%',left(ll.Fasteners,11),'%') set pp.参数号Process_NO=ll.Process_NO,pp.锪窝深度Countersink_depth=ll.Countersink_depth,pp.钻头转速Speed_of_drill=ll.Speed_of_drill,pp.给进速率Feed_speed=ll.Feed_speed,pp.夹紧力Clamp_force=ll.Clamp_force,pp.夹紧释放力Clamp_relief_force=ll.Clamp_relief_force,pp.墩铆力Upset_force=ll.Upset_force,pp.墩铆位置Upset_position=ll.Upset_position,pp.注胶压力Seal_pres=ll.Seal_pres,pp.注胶时间Seal_time=ll.Seal_time,pp.钻头Drill=ll.Drill,pp.上铆头Upper_Anvil=ll.Upper_Anvil,pp.胶嘴Sealant_Tip=ll.Tips";


            }
            else
        {
        sqlstr1 = "Update " + productnametrim + " pp inner join 紧固件列表 ll on ll.Fasteners like concat(pp.紧固件名称Fastener_Name,'%') set pp.参数号Process_NO=(case when pp.参数号Process_NO is null then ll.Process_NO else pp.参数号Process_NO end),pp.锪窝深度Countersink_depth=(case when pp.锪窝深度Countersink_depth is null then ll.Countersink_depth else pp.锪窝深度Countersink_depth end),pp.钻头转速Speed_of_drill=(case when pp.钻头转速Speed_of_drill is null then ll.Speed_of_drill else pp.钻头转速Speed_of_drill end),pp.给进速率Feed_speed=(case when pp.给进速率Feed_speed is null then ll.Feed_speed else pp.给进速率Feed_speed end),pp.夹紧力Clamp_force=(case when pp.夹紧力Clamp_force is null then ll.Clamp_force else pp.夹紧力Clamp_force end),pp.夹紧释放力Clamp_relief_force=(case when pp.夹紧释放力Clamp_relief_force is null then ll.Clamp_relief_force else pp.夹紧释放力Clamp_relief_force end),pp.墩铆力Upset_force=(case when pp.墩铆力Upset_force is null then ll.Upset_force else pp.墩铆力Upset_force end),pp.墩铆位置Upset_position=(case when pp.墩铆位置Upset_position is null then ll.Upset_position else pp.墩铆位置Upset_position end),pp.注胶压力Seal_pres=(case when pp.注胶压力Seal_pres is null then ll.Seal_pres else pp.注胶压力Seal_pres end),pp.注胶时间Seal_time=(case when pp.注胶时间Seal_time is null then ll.Seal_time else pp.注胶时间Seal_time end),pp.钻头Drill=ll.Drill,pp.上铆头Upper_Anvil=ll.Upper_Anvil,pp.胶嘴Sealant_Tip=ll.Tips";

                sqlstr = "Update " + productnametrim + " pp left join 紧固件列表 ll on pp.UUID like concat('%',left(ll.Fasteners,11),'%') set pp.参数号Process_NO=(case when pp.参数号Process_NO is null then ll.Process_NO else pp.参数号Process_NO end),pp.锪窝深度Countersink_depth=(case when pp.锪窝深度Countersink_depth is null then ll.Countersink_depth else pp.锪窝深度Countersink_depth end),pp.钻头转速Speed_of_drill=(case when pp.钻头转速Speed_of_drill is null then ll.Speed_of_drill else pp.钻头转速Speed_of_drill end),pp.给进速率Feed_speed=(case when pp.给进速率Feed_speed is null then ll.Feed_speed else pp.给进速率Feed_speed end),pp.夹紧力Clamp_force=(case when pp.夹紧力Clamp_force is null then ll.Clamp_force else pp.夹紧力Clamp_force end),pp.夹紧释放力Clamp_relief_force=(case when pp.夹紧释放力Clamp_relief_force is null then ll.Clamp_relief_force else pp.夹紧释放力Clamp_relief_force end),pp.墩铆力Upset_force=(case when pp.墩铆力Upset_force is null then ll.Upset_force else pp.墩铆力Upset_force end),pp.墩铆位置Upset_position=(case when pp.墩铆位置Upset_position is null then ll.Upset_position else pp.墩铆位置Upset_position end),pp.注胶压力Seal_pres=(case when pp.注胶压力Seal_pres is null then ll.Seal_pres else pp.注胶压力Seal_pres end),pp.注胶时间Seal_time=(case when pp.注胶时间Seal_time is null then ll.Seal_time else pp.注胶时间Seal_time end),pp.钻头Drill=ll.Drill,pp.上铆头Upper_Anvil=ll.Upper_Anvil,pp.胶嘴Sealant_Tip=ll.Tips";


            }

            //DbHelperSQL.ExecuteSql("Update " + productnametrim + " pp left join 试片列表 ll on pp.UUID=ll.程序段编号 and");
            //更新试片编号
            exesql.Add(sqlstr);
            exesql.Add(sqlstr1);
            exesql.AddRange(AutorivetDB.rfcouponno(prodname));
      //  DbHelperSQL.ExecuteSql("Update " + productnametrim + " pp left join (Select  程序段编号,GROUP_CONCAT(CONCAT('T',编号)) as 试片编号 from 试片列表 where 产品图号='"+ prodname+"' group by 程序段编号) ll on pp.UUID=ll.程序段编号 set pp.试片Coupon_used=ll.试片编号");

        DbHelperSQL.ExecuteSqlTran(exesql);

     //   DbHelperSQL.ExecuteSql(sqlstr);
    }



        private void button9_Click(object sender, EventArgs e)
        {
            DbHelperSQL.ExecuteSql("insert ignore into " + productnametrim + "(ID,uuid,加工位置location,紧固件名称Fastener_Name,紧固件数量Fastener_Qty,钻头Drill,下铆头Lower_Anvil,上铆头Upper_Anvil,胶嘴Sealant_Tip) (select ID,uuid,加工位置location,紧固件名称Fastener_Name,紧固件数量Fastener_Qty,钻头Drill,下铆头Lower_Anvil,上铆头Upper_Anvil,胶嘴Sealant_Tip from " + productnametrim +"_backup)");
       
         loadtable(productnametrim);
        }

        private void label5_Click(object sender, EventArgs e)
        {

        }

        private void button10_Click(object sender, EventArgs e)
        {
            if (comboBox4.SelectedIndex != -1 && comboBox5.SelectedIndex != -1)
            {
               // int skinthk = System.Convert.ToInt16(textBox2.Text);
                //int otherthk = System.Convert.ToInt16(textBox3.Text);
                string tempstr = comboBox4.Text + "," + comboBox5.Text;
                if (!listBox1.Items.Contains(tempstr))
                {
                    listBox1.Items.Add(tempstr);
                }
               
            }
        }

        private void button12_Click(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
        }

        private void button11_Click(object sender, EventArgs e)
        {


            if (comboBox2.SelectedIndex != -1  && comboBox3.SelectedIndex != -1 && listBox1.Items.Count != 0)
            {


                List<string> creatName = new List<string>();
                //处理加工类型
                string pptype="";
                if(comboBox3.SelectedIndex==0)
                {
                    pptype="FST";
                }
                else
                {
                    pptype="DRILL";
                }
                //判断下铆头

                string loweranvil = "";

                //如果是高锁或是只钻孔，则只有一种可能401D265

                if (comboBox1.Text.Contains("B020600") || pptype == "DRILL")
                {
                    loweranvil = "401D265";
                }
                else
                {
                    //铆钉在长桁上墩铆401D260
                    if(comboBox2.Text=="STR")
                    {
                        loweranvil = "401C260";
                    }
                    else
                    {
                        loweranvil = "401D263";
                    }

                }

                //生成程序段编号
                string ppno = comboBox2.Text + "_" + comboBox1.Text + "_" + pptype + "_" + comboBox4.Text;
                
                //处理试片

                //如果覆盖
                if(checkBox2.Checked==true)
                {
                    string delcoupon = "delete from 试片列表 where 产品图号='" + prodname + "' and 程序段编号='" + ppno + "';";
                    creatName.Add(delcoupon);
                }

                foreach(string cpun in listBox1.Items)
                {
                   string[] unstr= cpun.Split(',');
                   int skinthk = System.Convert.ToInt16(unstr[0]);
                   int otherthk = System.Convert.ToInt16(unstr[1]);
                   StringBuilder strSqlname = new StringBuilder();

                   strSqlname.Append("Insert ignore into 试片列表 (");

                   strSqlname.Append("蒙皮厚度,二层材料,二层厚度,产品图号,程序段编号");

                   strSqlname.Append(String.Format(") VALUES ({0},'{1}',{2},'{3}','{4}')", skinthk, comboBox2.Text, otherthk, prodname, ppno));

                    creatName.Add(strSqlname.ToString());


                }
                //重置试片编号
                //string sqlstrcp = "update 试片列表 aa left join (SELECT @rownum := @rownum + 1 as rank,产品图号,蒙皮厚度,二层厚度,totaltk from (SELECT  (二层厚度+蒙皮厚度) as totaltk,产品图号,蒙皮厚度,二层厚度 FROM 试片列表 where 产品图号='" + prodname + "' group by 蒙皮厚度,二层材料,二层厚度 order by totaltk) bb,(SELECT @rownum := 0) r) kk on aa.二层厚度=kk.二层厚度 and aa.蒙皮厚度=kk.蒙皮厚度 and aa.产品图号=kk.产品图号  set aa.编号=kk.rank";
            //    creatName.Add(sqlstrcp);
            
                //添加程序段
                int id = renum() + 1;
                this.getupdate(dt);

              //  if(checkBox2.Checked==false)
                //{

               
                string sqlstr = "Insert ignore into " + productnametrim + " (ID,UUID,加工位置location,紧固件名称Fastener_Name,下铆头Lower_Anvil) values (" + (renum() + 1).ToString() + ",'" + ppno + "','" + comboBox2.Text + "','" + comboBox1.Text + "','"+loweranvil+"');";               
                creatName.Add(sqlstr);

             //   }

                 try
                 {
                     DbHelperSQL.ExecuteSqlTran(creatName);
                  
                 }
                catch(Exception ee)
                 {
                     MessageBox.Show(ee.Message);
                 }
                 filother();
                   
                loadtable(productnametrim);


            }
            else
            {
                MessageBox.Show("请检查：\r\n1.是否未输入试片；\r\n2.是否没选择紧固件；\r\n3.是否没选择位置；\r\n4.是否没选择加工类型");

            }

        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void button13_Click(object sender, EventArgs e)
        {
            renum();

           this.getupdate(dt);

            loadtable(productnametrim);
        }

        private void panel2_Paint(object sender, PaintEventArgs e)
        {

        }

        private void button14_Click(object sender, EventArgs e)
        {
            DataTable tmpdt = (DataTable)dataGridView2.DataSource;
            OFFICE_Method.excelMethod.SaveDataTableToExcel(tmpdt);

        }

        //private  List<string> rfcouponno()
        //{
        //    List<string> sqllist = new List<string>();

        //    string sqlstrcp = "update 试片列表 aa inner join (SELECT @rownum := @rownum + 1 as rank,产品图号,蒙皮厚度,二层材料,二层厚度,totaltk from (SELECT  (二层厚度+蒙皮厚度) as totaltk,产品图号,蒙皮厚度,二层材料,二层厚度 FROM 试片列表 where 产品图号='" + prodname + "' group by 蒙皮厚度,二层材料,二层厚度 order by totaltk) bb,(SELECT @rownum := 0) r) kk on aa.二层厚度=kk.二层厚度 and aa.蒙皮厚度=kk.蒙皮厚度 and aa.二层材料=kk.二层材料 and aa.产品图号=kk.产品图号  set aa.编号=kk.rank";
        //    sqllist.Add(sqlstrcp);

        //    sqllist.Add("Update " + productnametrim + " pp left join (Select  程序段编号,GROUP_CONCAT(CONCAT('T',编号)) as 试片编号 from 试片列表 where 产品图号='" + prodname + "' group by 程序段编号) ll on pp.UUID=ll.程序段编号 set pp.试片Coupon_used=ll.试片编号");

        //    return sqllist;
        //}
        private void button15_Click(object sender, EventArgs e)
        {
          //  string sqlstrcp = "update 试片列表 aa left join (SELECT @rownum := @rownum + 1 as rank,产品图号,蒙皮厚度,二层厚度,totaltk from (SELECT  (二层厚度+蒙皮厚度) as totaltk,产品图号,蒙皮厚度,二层厚度 FROM 试片列表 where 产品图号='" + prodname + "' group by 蒙皮厚度,二层厚度 order by totaltk) bb,(SELECT @rownum := 0) r) kk on aa.二层厚度=kk.二层厚度 and aa.蒙皮厚度=kk.蒙皮厚度 and aa.产品图号=kk.产品图号  set aa.编号=kk.rank";
          //  DbHelperSQL.ExecuteSql(rfcouponno());

            DbHelperSQL.ExecuteSqlTran(AutorivetDB.rfcouponno(prodname));

          //  DbHelperSQL.ExecuteSql("Update " + productnametrim + " pp left join (Select  程序段编号,GROUP_CONCAT(CONCAT('T',编号)) as 试片编号 from 试片列表 where 产品图号='" + prodname + "' group by 程序段编号) ll on pp.UUID=ll.程序段编号 set pp.试片Coupon_used=ll.试片编号");


            loadtable(productnametrim);
        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
        }

        private void button16_Click(object sender, EventArgs e)
        {
            DataTable tmpdt = DbHelperSQL.Query("select * from 试片列表 where 产品图号='" + prodname + "'").Tables[0];

            OFFICE_Method.excelMethod.SaveDataTableToExcel(tmpdt);
           // excelMethod.SaveDataTableToExcel(tmpdt);
        }

        private void button17_Click(object sender, EventArgs e)
        {

        }

        private void button18_Click(object sender, EventArgs e)
        {
            int row = dataGridView1.SelectedRows[0].Index;
            DataRow insertrow = ((DataRowView)dataGridView1.SelectedRows[0].DataBoundItem).Row;
            string uuid = insertrow["UUID"].ToString();
            var item = uuid.Split('_');
            string location = item[0];
            string fstname = item[1];
            string proctype = item[2];
            string skinthk = item[3];

            comboBox2.Text = location;
            comboBox1.Text = fstname;
            comboBox3.Text = proctype;
            comboBox4.Text = skinthk;

            //试片列表
            // DataTable tmpdt = DbHelperSQL.Query("select * from 试片列表 where 产品图号='" + prodname + "'").Tables[0];
        }

        private void button19_Click(object sender, EventArgs e)
        {
            int row = dataGridView1.SelectedRows[0].Index;
            DataRow insertrow = ((DataRowView)dataGridView1.SelectedRows[0].DataBoundItem).Row;
            string uuid = insertrow["UUID"].ToString();
            var item = uuid.Split('_');
            string location = item[0];
            string fstname = item[1];
            string proctype = item[2];
            string skinthk = item[3];

            string location1 = comboBox2.Text;
            string fstname1 = comboBox1.Text;
            string proctype1 = comboBox3.Text;
            string skinthk1 = comboBox4.Text;

            string uuid1 = location1 + "_" + fstname1 + "_" + proctype1 + "_" + skinthk1;

            if (textBox2.Text != "")
            {
                uuid1 = textBox2.Text;
                    }

            if (uuid!=uuid1)
            {
                var kk = from pp in dt.AsEnumerable()
                         select pp["UUID"].ToString();

                if (!kk.Contains(uuid1))
                {
                   
                    dataGridView1.Rows[row].Cells["UUID"].Value = uuid1;
                    dataGridView1.Rows[row].Cells["加工位置location"].Value = location1;

                    dataGridView1.Rows[row].Cells["紧固件名称Fastener_Name"].Value = fstname1;
                    //替换试片
                   dt.Rows[row]["UUID"] = uuid1;
                    dt.Rows[row]["加工位置location"]= location1;

                  dt.Rows[row]["紧固件名称Fastener_Name"] = fstname1;

                    dt.Rows[row].EndEdit();
                   
                    this.getupdate(dt);
                    DbHelperSQL.ExecuteSql("update 试片列表 set 程序段编号='" + uuid1 + "' where 产品图号 = '" + prodname + "' and 程序段编号='" + uuid + "'");

                }
                else

                {
                    MessageBox.Show("修改后的UUID与其他的重名！");
                }

            }



         


           






        }

        private void button20_Click(object sender, EventArgs e)
        {
            int row = dataGridView2.SelectedRows[0].Index;
            DataRow insertrow = ((DataRowView)dataGridView2.SelectedRows[0].DataBoundItem).Row;

            // int row = dataGridView1.SelectedRows.Count;
            //  if (MessageBox.Show("确认删除第" + row.ToString() + "条记录吗？", "请确认", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            //   {
            try
            {
               var cpno=  System.Convert.ToInt32( insertrow["组合编号"].ToString().Remove(0, 1));
                string delcoupon = "delete from 试片列表 where 产品图号='" + prodname + "' and 编号=" + cpno + ";";
                DbHelperSQL.ExecuteSql(delcoupon);
                DbHelperSQL.ExecuteSqlTran(AutorivetDB.rfcouponno(prodname));

            }
            catch
            {

                MessageBox.Show("删除时发生错误，请注意编号");
            }
            finally
            {

                loadtable(productnametrim);
            }

            /*
              if (row>1)
              {
                  dataGridView1.Rows[row - 1].Selected = true;
              }
              else
              {
                  dataGridView1.Rows[row + 1].Selected = true;
              }
             * */
          
           
        }
    }
}
