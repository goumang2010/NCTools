using Microsoft.VisualStudio.TestTools.UnitTesting;
using NC_TOOL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NC_TOOL.Tests
{
    [TestClass]
    public class pointCoordListTests
    {
        [TestMethod()]
        public void OutPutTest()
        {
            var testlist = new pointCoordList();
            testlist.Add(new pointCoord("X;1;Y;0;"));
            testlist.Add(new pointCoord("C;90;"));
           string NCcode= testlist.OutPut();
            Assert.AreEqual(NCcode, "X1Y0Z0W0A0\r\nG01C0090\r\n");
        }




    }
}