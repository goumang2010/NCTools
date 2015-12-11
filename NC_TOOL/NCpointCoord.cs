using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace NC_TOOL
{
  public  class NCpointCoord:pointCoord
    {
        public int RowNum
        {
            get;
            set;
        }
        public int PFNum
        {
            get;
            set;
        }
        public string PFName
        {
            get;
            set;
        }
        public string Geoset
        {
            get;
            set;
        }

        public string Operation
        {
            get;
            set;
        }
        public string UUID
        {
            get;
            set;

        }
        public NCpointCoord(string NCCoordString)
        {
            Regex regex = new Regex(@"[A-Z]");
            var coord = regex.Split(NCCoordString);
            
            X = coord[1];
            Y = coord[2];
            Z = coord[3];
            W= coord[4];
            A= coord[5];
            UUID = Regex.Replace(NCCoordString, @"\.[0-9]*[A-Z]", "_", RegexOptions.None).Split('.')[0].Substring(1);
            
        }

        public NCpointCoord()
        {



        }

        public string OutPut()
        {
            return this.ToString() + "," +PFName+","+PFNum.ToString()+","+ Operation + ","+ Geoset + ","+ RowNum.ToString();
        }


        public string[] ToArray()
        {
            return new string[] { this.ToString() , PFName , PFNum.ToString(), Operation,Geoset , RowNum.ToString() };
        }


    }
}
