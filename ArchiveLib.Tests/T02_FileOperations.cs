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
            string archiveFilePath = ConstantsPR.PATH_ARCHIVE + @"\t.txt";
            Archive archive = new Archive(ConstantsPR.UserDomain, ConstantsPR.UserName, ConstantsPR.UserPassword);

            FileInfo info = archive.GetFileInfo(archiveFilePath);
            Assert.AreEqual(6, info.Length, "B user should be able to read file info.");
        }

        [TestMethod]
        public void Copy_To_Archive()
        {
            string sourceFilePath = ConstantsPR.PATH_COMMON + @"\CommonFile.txt";
            string archiveFilePath = ConstantsPR.PATH_ARCHIVE + @"\CommonFile.txt";

            Archive archive = new Archive(ConstantsPR.UserDomain, ConstantsPR.UserName, ConstantsPR.UserPassword);

            archive.CopyFileTo(sourceFilePath, archiveFilePath);
            FileInfo info = archive.GetFileInfo(archiveFilePath);
            Assert.AreEqual(true, info.Exists, "File has not been copied to archive.");

            info = archive.GetFileInfo(archiveFilePath + ".nofile");
            Assert.AreEqual(false, info.Exists, "No existing file found in archive.");
        }

        [TestMethod]
        public void Copy_From_Archive()
        {
            string sourceFilePath = ConstantsPR.PATH_ARCHIVE + @"\t.txt";
            string destinationFilePath = ConstantsPR.PATH_COMMON + @"\t.fromarchive.txt";

            Archive archive = new Archive(ConstantsPR.UserDomain, ConstantsPR.UserName, ConstantsPR.UserPassword);
            archive.CopyFileFrom(sourceFilePath, destinationFilePath);

            FileInfo info =new FileInfo(destinationFilePath);
            Assert.AreEqual(true, info.Exists, "File has not been copied from archive.");

            if (info.Exists)
            {
                File.Delete(destinationFilePath);
            }          
        }

        [TestMethod]
        public void Delete_In_Archive()
        {
            string sourceFilePath = ConstantsPR.PATH_COMMON + @"\CommonFile.txt";
            string archiveFilePath = ConstantsPR.PATH_ARCHIVE + @"\CommonFile.txt";

            Archive archive = new Archive(ConstantsPR.UserDomain, ConstantsPR.UserName, ConstantsPR.UserPassword);

            archive.CopyFileTo(sourceFilePath, archiveFilePath);
            FileInfo info = archive.GetFileInfo(archiveFilePath);
            Assert.AreEqual(true, info.Exists, "File has not been copied to archive.");

            archive.DeleteFile(archiveFilePath);
            info = archive.GetFileInfo(archiveFilePath);
            Assert.AreEqual(false, info.Exists, "File has not been deleted from archive.");

        }

        //[TestMethod]
        //public void Archived_FileInfoNegative()
        //{
        //    string archiveFilePath = ConstantsPR.PATH_ARCHIVE + @"\t.txt";
        //    using (Archive archive = new Archive(ConstantsPR.UserDomain, ConstantsPR.UserName, ConstantsPR.UserPassword))
        //    {
        //        FileInfo info = archive.GetFileInfo(archiveFilePath);
        //        Assert.AreEqual(info.Length > 0, true);
        //    }
        //}
    }
}