using Microsoft.VisualStudio.TestTools.UnitTesting;
using NC_TOOL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NC_TOOL.Tests
{
    [TestClass()]
    public class NCcoordTests
    {
        [TestMethod()]
        public void NCpointCoordTest()
        {
          //  Assert.Fail();
        }

        [TestMethod()]
        public void NCpointCoordTest1()
        {
            NCpointCoord pt = new NCpointCoord("X8907.9760Y-3158.7913Z917.2739W-1825.5808A-4.5928");

            Assert.AreEqual(pt.Cstate, false);
            Assert.AreEqual(pt.ToString(), "X8907.9760Y-3158.7913Z917.2739W-1825.5808A-4.5928");
            Assert.AreEqual(pt.UUID, "8907_-3158_917_-1825_-4");
        }


    }
}