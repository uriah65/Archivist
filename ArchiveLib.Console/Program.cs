using System.Diagnostics;

namespace ArchiveLib.Console
{
    internal class Program
    {
        //private const string largefileName = "large.iso";
        //private const string sourceFilePath = @"C:\temp\" + largefileName;

        //private const string archiveFilePath = ConstantsPR.PATH_ARCHIVE + @"\" + largefileName;

        private static void Main(string[] args)
        {
            string sourceFilePath = @"C:\temp\large.iso";
            string destinationFilePath = @"C:\temp\largeNew.iso";

            string path = @"C:\temp\credentials.txt";
            string[] lines = System.IO.File.ReadAllLines(path);
            string userDomain = lines[0];
            string userName = lines[1];
            string userPassword = lines[2];

            Stopwatch sw = new Stopwatch();
            sw.Start();

           

            Archive archive = new Archive(userDomain, userName, userPassword);
            archive.CopyWithThreads(sourceFilePath, destinationFilePath, true);

            System.Console.WriteLine("Done 1: " + sw.ElapsedMilliseconds / 1000.0);
            System.Console.ReadKey();


            archive = new Archive(userDomain, userName, userPassword);
            archive.CopyToArchive_MemoryFile(sourceFilePath, destinationFilePath);

            System.Console.WriteLine("Done 2: " + sw.ElapsedMilliseconds / 1000.0);
            System.Console.ReadKey();
        }
    }
}