using Microsoft.VisualStudio.TestTools.UnitTesting;
using NC_TOOL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NC_TOOL.Tests
{
    [TestClass()]
    public class FasternersRepoNCcoordTests
    {
        [TestMethod()]
        public void getFasternerTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        [ExpectedException(typeof(NCException), "没有这种紧固件！")]
        public void getFastNameTest()
        {
           
            FasternersRepo dd = new FasternersRepo();
            dd.Add(new Fasterner() { FasternerName = "aaa", TCode = 111 });
            dd.Add(new Fasterner() { FasternerName = "bbb", TCode =222 });

            Assert.AreEqual(dd.getFastName(111), "aaa");
            dd.getFastName(444);
           

         
        }

        [TestMethod()]
        public void getQTYTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void getResyncCodeTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void getTCodeTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void GetListTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void MergeTest()
        {
            Assert.Fail();
        }

        [TestMethod()]
        public void MergeTest1()
        {
            Assert.Fail();
        }
    }
}