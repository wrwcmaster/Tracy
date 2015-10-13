using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Tracy.DataModel;

namespace TracyTest
{
    [TestClass]
    public class ModelTest
    {
        [TestMethod]
        public void TestEpisode()
        {
            var file = new MediaFile() { FileName = "[KTXP][Aldnoah.Zero][15][GB][720p].mp4" };
            Assert.AreEqual(15, file.Episode);
            file = new MediaFile() { FileName = "[KTXP][Aldnoah.Zero][00][GB][720p].mp4" };
            Assert.AreEqual(0, file.Episode);
            file = new MediaFile() { FileName = "[KTXP][Aldnoah.Zero][][GB][720p].mp4" };
            Assert.AreEqual(null, file.Episode);
        }
    }
}
