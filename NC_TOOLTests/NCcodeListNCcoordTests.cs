using Microsoft.VisualStudio.TestTools.UnitTesting;
using NC_TOOL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NC_TOOL.Tests
{
    [TestClass()]
    public class NCcodeListNCcoordTests
    {
        [TestMethod()]
        public void CheckPointsTest()
        {
           var ls= ImportFromFileTest();
            ls.Check(true, true, false);
            Assert.AreEqual(ls.wronglist.Count(), 128);
        }

        [TestMethod()]
        public NCcodeList ImportFromFileTest()
        {
           string testpath= System.Environment.CurrentDirectory + @"\TestFiles\WRONG_NC";
            NinjectDependencyResolver dd = new NinjectDependencyResolver();
            
            NCcodeList ncls = new NCcodeList((IDBInfo)dd.GetService(typeof(IDBInfo)));

            ncls.ImportFromFile(testpath);
            Assert.AreNotEqual(ncls.Count(),0);
            return ncls;
        }
    }
}