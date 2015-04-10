using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace ArchiveLib.Tests
{
    [TestClass]
    public class T03_LargeFiles
    {
        private const string largefileName = "large.iso";
        private const string sourceFilePath = @"C:\temp\" + largefileName;

        private const string archiveFilePath = ConstantsPR.PATH_ARCHIVE + @"\" + largefileName;

        [TestMethod]
        public void Copy_To_Archive_Large()
        {
            long filelength = (new FileInfo(sourceFilePath)).Length;

            Archive archive = new Archive(ConstantsPR.UserDomain, ConstantsPR.UserName, ConstantsPR.UserPassword);

            archive.CopyToArchiveLarge(sourceFilePath, archiveFilePath);
            FileInfo info = archive.GetFileInfo(archiveFilePath);
            Assert.AreEqual(filelength, info.Length, "File has not been copied to archive.");

            archive.DeleteInArchive(archiveFilePath);
        }


    }
}