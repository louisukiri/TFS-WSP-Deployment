using System;
using System.IO;
using Deployers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using toDev.Concrete;

namespace Deployer_Test
{
    [TestClass]
    public class UnitTest1
    {
        public UnitTest1()
        {
            TFSDeployerController depController = new TFSDeployerController("http://team:8080/tfs/WebSites", "baseDir", string.Empty, "1");
        }
        [TestMethod]
        public void DefaultObject_stream_test()
        {
            Stream fs = DefaultFileObject.get("$/B2B/Websites/CBG/Dev/_test-branch/jj.txt") as Stream;
            bool a = fs == null;
            if(fs != null)
                fs.Close();

            Assert.IsFalse(a);
        }
        [TestMethod]
        public void DefaultObject_TFSConfig_Test()
        {
            var a = BuildConfiguration.GetConfig("$/B2B/Websites/CBG/Dev/");
            var b = BuildConfiguration.GetConfig("$/B2B/Websites/CBG/Dev/build.config");

            Assert.IsTrue(a.Rules.Count > 0);
            Assert.IsTrue(b.Rules.Count > 0);
        }
    }
}
