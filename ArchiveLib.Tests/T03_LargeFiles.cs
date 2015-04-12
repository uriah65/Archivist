using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
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
            Stopwatch sw = new Stopwatch();
            sw.Start();

            long filelength = (new FileInfo(sourceFilePath)).Length;

            Archive archive = new Archive(ConstantsPR.UserDomain, ConstantsPR.UserName, ConstantsPR.UserPassword);

            archive.CopyToArchiveLarge(sourceFilePath, archiveFilePath);
            FileInfo info = archive.GetFileInfo(archiveFilePath);
            Assert.AreEqual(filelength, info.Length, "File has not been copied to archive.");

            archive.DeleteInArchive(archiveFilePath);

            Debug.WriteLine("Done: " + sw.ElapsedMilliseconds / 1000.0);
        }

        [TestMethod]
        public void Copy_To_Archive_Threads()
        {
            Stopwatch sw = new Stopwatch();
            sw.Start();

            long filelength = (new FileInfo(sourceFilePath)).Length;

            Archive archive = new Archive(ConstantsPR.UserDomain, ConstantsPR.UserName, ConstantsPR.UserPassword);

            archive.CopyToArchiveThreads(sourceFilePath, archiveFilePath);
            FileInfo info = archive.GetFileInfo(archiveFilePath);
            Assert.AreEqual(filelength, info.Length, "File has not been copied to archive.");

            archive.DeleteInArchive(archiveFilePath);

            Debug.WriteLine("Done: " + sw.ElapsedMilliseconds / 1000.0);
        }

    }
}