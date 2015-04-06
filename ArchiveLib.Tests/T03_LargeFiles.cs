using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;

namespace ArchiveLib.Tests
{
    [TestClass]
    public class T03_LargeFiles
    {
        [TestMethod]
        public void Copy_To_Archive_Large()
        {
            string largefileName = "large.iso";
            string sourceFilePath = @"C:\temp\" + largefileName;
            string archiveFilePath = ConstantsPR.PATH_ARCHIVE + @"\" + largefileName;

            long ln = (new FileInfo(sourceFilePath)).Length;

            Archive archive = new Archive(ConstantsPR.UserDomain, ConstantsPR.UserName, ConstantsPR.UserPassword);

            archive.CopyToArchiveLarge(sourceFilePath, archiveFilePath);
            FileInfo info = archive.GetFileInfo(archiveFilePath);
            Assert.AreEqual(ln, info.Length, "File has not been copied to archive.");

            archive.DeleteInArchive(archiveFilePath);

        }
    }
}
