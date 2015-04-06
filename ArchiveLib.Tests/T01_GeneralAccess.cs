using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace ArchiveLib.Tests
{
    [TestClass]
    public class T01_GeneralAccess
    {
        private string _b_path = @"..\..\BOnly";
        private string _a_path = @"..\..\AOnly";


        [TestMethod]
        [ExpectedException(typeof(System.ApplicationException))]
        public void Invalid_Credentials()
        {
            using (Archive archive = new Archive(ConstantsPR.UserDomain, ConstantsPR.UserName, "XXXXX"))
            {
                archive.WrapAction(() => Act(_a_path));
            }
        }

        [TestMethod]
        public void User_Access_To_A()
        {
            using (Archive archive = new Archive(ConstantsPR.UserDomain,  ConstantsPR.UserName, ConstantsPR.UserPassword))
            {
                archive.WrapAction(() => Act(_a_path));
            }
        }

        [TestMethod]
        [ExpectedException(typeof(System.UnauthorizedAccessException))]
        public void User_No_Access_To_B()
        {
            using (Archive archive = new Archive(ConstantsPR.UserDomain, ConstantsPR.UserName, ConstantsPR.UserPassword))
            {
                archive.WrapAction(() => Act(_b_path));
            }
        }

        [TestMethod]
        public void Default_Access_To_A()
        {
            Act(_b_path);
        }

        [TestMethod]
        public void Default_Access_To_B()
        {
            Act(_b_path);
        }

        private object Act(string path)
        {
            path = path + @"\t.txt";
            FileInfo info = new FileInfo(path);
            return info.Length;
        }
    }
}