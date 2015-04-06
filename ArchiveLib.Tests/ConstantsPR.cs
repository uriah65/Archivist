namespace ArchiveLib.Tests
{
    public static class ConstantsPR
    {
        public const string PATH_ARCHIVE = @"..\..\Archive";
        public const string PATH_COMMON = @"..\..\Common";

        public static string UserDomain { get; private set; }

        public static string UserName { get; private set; }

        public static string UserPassword { get; private set; }

        static ConstantsPR()
        {
            string path = @"C:\temp\credentials.txt";
            string[] lines = System.IO.File.ReadAllLines(path);
            UserDomain = lines[0];
            UserName = lines[1];
            UserPassword = lines[2];
        }
    }
}