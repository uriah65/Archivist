using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace ArchiveLib.Tests
{
    [TestClass]
    public class T01_GeneralAccess
    {
        [TestMethod]
        [ExpectedException(typeof(System.ApplicationException))]
        public void Invalid_Credentials()
        {
            using (Archive archive = new Archive(ConstantsPR.UserDomain, ConstantsPR.UserName, "XXXXX"))
            {
                CheckFile(ConstantsPR.PATH_COMMON);
            }
        }

        [TestMethod]
        public void B_User_Access_Check()
        {
            using (Archive archive = new Archive(ConstantsPR.UserDomain, ConstantsPR.UserName, ConstantsPR.UserPassword))
            {
                long ln = CheckFile(ConstantsPR.PATH_COMMON);
                Assert.AreEqual(9, ln, "B user have access to a common directory.");
                ln = CheckFile(ConstantsPR.PATH_ARCHIVE);
                Assert.AreEqual(6, ln, "B user have access to Archive directory.");
            }
        }

        [TestMethod]
        [ExpectedException(typeof(System.UnauthorizedAccessException))]
        public void Current_User_Access_To_Archive()
        {
            // current user have no access to archive
            long ln = CheckFile(ConstantsPR.PATH_ARCHIVE);
            Assert.AreEqual(9, ln, "Current user have NO access to Archive directory.");
        }

        [TestMethod]
        public void Current_User_Acces_To_Common()
        {
            long ln = CheckFile(ConstantsPR.PATH_COMMON);
            Assert.AreEqual(9, ln, "Current user have access to a common directory.");
        }


        private long CheckFile(string path)
        {
            path = path + @"\t.txt";
            FileInfo info = new FileInfo(path);
            return info.Length;
        }
    }
}