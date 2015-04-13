using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using System.IO;

namespace ArchiveLib.Tests
{
    [TestClass]
    public class T03_LargeFiles
    {
        //private const string largefileName = "large.iso";
        //private const string sourceFilePath = @"C:\temp\" + largefileName;

        //private const string largefileName = "autumn.jpg";
        private const string sourceFilePath = @"C:\temp\autumn.jpg";


        //private const string archiveFilePath = ConstantsPR.PATH_ARCHIVE + @"\" + largefileName;
        private const string archiveFilePath = @"C:\temp\autumnNew.jpg";

        [TestMethod]
        public void Copy_To_Archive_Large()
        {
            return; 

            Stopwatch sw = new Stopwatch();
            sw.Start();

            long filelength = (new FileInfo(sourceFilePath)).Length;

            Archive archive = new Archive(ConstantsPR.UserDomain, ConstantsPR.UserName, ConstantsPR.UserPassword);

            archive.CopyToArchive_MemoryFile(sourceFilePath, archiveFilePath);
            FileInfo info = archive.GetFileInfo(archiveFilePath);
            Assert.AreEqual(filelength, info.Length, "File has not been copied to archive.");

            archive.DeleteInArchive(archiveFilePath);

            Debug.WriteLine("Done: " + sw.ElapsedMilliseconds / 1000.0);
        }

        [TestMethod]
        public void Copy_To_Archive_Threads()
        {
            return;

            Stopwatch sw = new Stopwatch();
            sw.Start();

            long filelength = (new FileInfo(sourceFilePath)).Length;

            Archive archive = new Archive(ConstantsPR.UserDomain, ConstantsPR.UserName, ConstantsPR.UserPassword);

            archive.CopyWithThreads(sourceFilePath, archiveFilePath, true);
            FileInfo info = archive.GetFileInfo(archiveFilePath);
            Assert.AreEqual(filelength, info.Length, "File has not been copied to archive.");

            archive.DeleteInArchive(archiveFilePath);

            Debug.WriteLine("Done: " + sw.ElapsedMilliseconds / 1000.0);
        }

    }
}