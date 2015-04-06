using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace ArchiveLib.Tests
{
    [TestClass]
    public class T02_FileOperations
    {
        [TestMethod]
        public void Archived_FileInfo()
        {
           string archiveFilePath = ConstantsPR.PATH_BONLY + @"\t.txt";
           using (Archive archive = new Archive(ConstantsPR.UserDomain, ConstantsPR.UserName, ConstantsPR.UserPassword))
           {
                FileInfo info = archive.GetFileInfo(archiveFilePath);
                Assert.AreEqual(info.Length > 0, true);
            }
        }

        [TestMethod]
        public void Archived_FileInfoNegative()
        {
            string archiveFilePath = ConstantsPR.PATH_BONLY + @"\t.txt";
            using (Archive archive = new Archive(ConstantsPR.UserDomain, ConstantsPR.UserName, ConstantsPR.UserPassword))
            {
                FileInfo info = archive.GetFileInfo(archiveFilePath);
                Assert.AreEqual(info.Length > 0, true);
            }
        }

    }
}
