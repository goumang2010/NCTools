using Microsoft.VisualStudio.TestTools.UnitTesting;
using NC_TOOL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NC_TOOL.Tests
{
    [TestClass()]
    public class DBInfoNCcoordTests
    {
        [TestMethod()]
        public void DBInfoTest()
        {
            DBInfo dbi = new DBInfo();
            Assert.AreNotEqual(dbi.DBfstTable.Count(), 0);
        }
    }
}