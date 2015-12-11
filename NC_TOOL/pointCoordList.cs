using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using mysqlsolution;


namespace NC_TOOL
{
 

        public  class pointCoordList:List<pointCoord>
        {


            public string OutPut( string afterpoint)
            {

                string newtext = "";


              ForEach(delegate (pointCoord pp)
                {

                    newtext = newtext + pp.ToString() + "\r\n" + afterpoint + "\r\n";



                });
                return newtext;

            }
        public string OutPut()
        {

            string newtext = "";


            ForEach(delegate (pointCoord pp)
            {

                newtext = newtext + pp.ToString() + "\r\n" ;



            });
            return newtext;

        }


        public  IEnumerable<string> ToList()
            {
                foreach (var pp in this)
                {
                    yield return pp.ToString();
                }


            }



     public pointCoordList(string filepath)

        {
            ImportFromFile(filepath);
        }

     public   pointCoordList()

        {
           
        }


        public List<pointCoord> ImportFromFile(string filepath)
            {
        

                var tmplist = from x in localMethod.ReadLines(filepath)
                              let y = x.Trim()
                              where y != "" && (!y.Contains("#"))
                              select y;




                pointCoord tmpt = new pointCoord(tmplist.First());
                Add(tmpt);

                for (int i = 1; i < tmplist.Count(); i++)
                {

                   Add(new pointCoord(this[i - 1], tmplist.ElementAt(i)));


                }

                return this;

            }










        }
    }

