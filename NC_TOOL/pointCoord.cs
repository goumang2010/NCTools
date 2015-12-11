using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NC_TOOL
{
    public class pointCoord
    {
        public pointCoord preCoord;
        private string x;
        private string y;
        private string z;
        private string w;
        private string a;
        private string c;


        public string X
        {
            get
            {


                return getCoord("x");


            }
            set
            {
                x = value;

            }

        }
        public string Y
        {
            get
            {

                return getCoord("y");
            }
            set
            {
                y = value;

            }

        }

        public string Z
        {
            get
            {

                return getCoord("z");
            }
            set
            {
                z = value;

            }

        }
        public string W
        {
            get
            {

                return getCoord("w");
            }
            set
            {
                w = value;

            }

        }
        public string A
        {
            get
            {

                return getCoord("a");
            }
            set
            {
                a = value;

            }


        }
        public string C
        {
            get
            {

                return getCoord("c");
            }
            set
            {
                Cstate = true;
                c = value;

            }


        }
        public bool Cstate
        {
            get;
            set;

        }

        private string getCoord(string coordName)

        {
            BindingFlags flag = BindingFlags.Instance | BindingFlags.NonPublic;
            //   BindingFlags flag2 = BindingFlags.Instance | BindingFlags.Public;
            Type type = typeof(pointCoord);
            FieldInfo field = type.GetField(coordName, flag);
            string tmpstr = (string)field.GetValue(this);
            //   x = x ?? (preCoord.X ?? "0");
            if (tmpstr != null)
            {

                // return tmpstr;
            }
            else
            {
                if (preCoord != null)
                {
                    Type type2 = preCoord.GetType();
                    PropertyInfo property = type2.GetProperty(coordName.ToUpper());
                    tmpstr = (string)property.GetValue(preCoord, null);




                }
                else
                {
                    tmpstr = "0";
                }

                field.SetValue(this, tmpstr);

            }

            return tmpstr;
        }


        public pointCoord(string tabStr)
        {
            iniCoordFromTab(tabStr);



        }
        public pointCoord()
        {

        }

        public void iniCoordFromTab(string tabStr)
        {
            var tmp = tabStr.Split(';');

            int count = tmp.Count() / 2;

            Type type = this.GetType(); //获取类型
            for (int i = 0; i < count; i++)
            {
                System.Reflection.PropertyInfo propertyInfo = type.GetProperty(tmp[2 * i]);

                propertyInfo.SetValue(this, tmp[2 * i + 1], null); //给对应属性赋值

            }
        }
        public void iniCoordFromNC(string tabStr)
        {
            var tmp = tabStr.Split(';');

            int count = tmp.Count() / 2;

            Type type = this.GetType(); //获取类型
            for (int i = 0; i < count; i++)
            {
                System.Reflection.PropertyInfo propertyInfo = type.GetProperty(tmp[2 * i]);

                propertyInfo.SetValue(this, tmp[2 * i + 1], null); //给对应属性赋值

            }
        }
        public pointCoord(pointCoord pre)
        {
            this.preCoord = pre;



        }


        public pointCoord(pointCoord pre, string tabStr)
        {
            preCoord = pre;
            var tmp = tabStr.Split(';');

            int count = tmp.Count() / 2;

            Type type = this.GetType(); //获取类型
            for (int i = 0; i < count; i++)
            {
                System.Reflection.PropertyInfo propertyInfo = type.GetProperty(tmp[2 * i]);

                propertyInfo.SetValue(this, tmp[2 * i + 1], null); //给对应属性赋值

            }

            if (tabStr.Contains("C"))
            {
                Cstate = true;

            }
            else
            {
                Cstate = false;
            }




        }

        public override string ToString()
        {
            string outputstr;
            if (Cstate == true)
            {
                outputstr = "G01C0" + C.ToString().PadLeft(3,'0');
            }
            else
            {
                outputstr = "X" + X.ToString() + "Y" + Y.ToString() + "Z" + Z.ToString() + "W" + W.ToString() + "A" + A.ToString();
            }




            return outputstr;

        }

        public pointCoord Offset(string offsetStr)
        {
            pointCoord offsetCooord = new pointCoord(this);
            var tmp = offsetStr.Split(';');
            int count = tmp.Count() / 2;

            Type type = this.GetType(); //获取类型
            for (int i = 0; i < count; i++)
            {
                System.Reflection.PropertyInfo propertyInfo = type.GetProperty(tmp[2 * i]);
                string basecoordStr = (string)propertyInfo.GetValue(this, null);
                //Convert it to double
                double offset = System.Convert.ToDouble(tmp[2 * i + 1]);
                double basecoord = System.Convert.ToDouble(basecoordStr);
                propertyInfo.SetValue(offsetCooord, (basecoord + offset).ToString(), null); //给对应属性赋值

            }
            //  offsetCooord.preCoord = this;
            return offsetCooord;

        }









    }
}
