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
                archive.WrapAction(() => Act(ConstantsPR.PATH_COMMON));
            }
        }

        [TestMethod]
        public void User_Access_To_Common()
        {
            using (Archive archive = new Archive(ConstantsPR.UserDomain, ConstantsPR.UserName, ConstantsPR.UserPassword))
            {
                archive.WrapAction(() => Act(ConstantsPR.PATH_COMMON));
            }
        }

        [TestMethod]
        [ExpectedException(typeof(System.UnauthorizedAccessException))]
        public void User_Access_To_Archive()
        {
            using (Archive archive = new Archive(ConstantsPR.UserDomain, ConstantsPR.UserName, ConstantsPR.UserPassword))
            {
                archive.WrapAction(() => Act(ConstantsPR.PATH_ARCHIVE));
            }
        }

        [TestMethod]
        public void Default_Access_To_A()
        {
            Act(ConstantsPR.PATH_COMMON);
        }

        [TestMethod]
        public void Default_Access_To_B()
        {
            Act(ConstantsPR.PATH_ARCHIVE);
        }

        private object Act(string path)
        {
            path = path + @"\t.txt";
            FileInfo info = new FileInfo(path);
            return info.Length;
        }
    }
}