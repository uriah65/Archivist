using Microsoft.Win32.SafeHandles;
using System;
using System.IO;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Principal;
using System.Threading;

namespace ProducerConsumerFileConsole
{
    // https://msdn.microsoft.com/en-us/library/aa645740%28v=vs.71%29.aspx
    internal class Program
    {
        public static void Main(String[] args)
        {
            int result = 0;   // Result initialized to say there is no error

            string sourceFilePath = @"C:\temp\autumn.jpg";
            string destinationFilePath = @"C:\temp\autumnNew.jpg";

            string credentialsFilePath = @"C:\temp\credentials.txt";
            string[] lines = System.IO.File.ReadAllLines(credentialsFilePath);

            Login login = new Login();
            login.Domain = lines[0];
            login.Account = lines[1];
            login.Password = lines[2];

            int boxSize = (int) Math.Pow(2, 20); //1000000; // 1MB


            ByteBox byteBox = new ByteBox();

            FileReader reader = new FileReader(byteBox, boxSize, sourceFilePath);  
            FileWriter writer = new FileWriter(byteBox, destinationFilePath, login);  

            Thread readerThread = new Thread(new ThreadStart(reader.ThreadRun));
            Thread writerThread = new Thread(new ThreadStart(writer.ThreadRun));

            try
            {
                readerThread.Start();
                writerThread.Start();

                readerThread.Join();   // Join both threads with no timeout
                                       // Run both until done.
                writerThread.Join();
                // threads producer and consumer have finished at this point.
            }
            catch (ThreadStateException e)
            {
                Console.WriteLine(e);  // Display text of exception
                result = 1;            // Result says there was an error
            }
            catch (ThreadInterruptedException e)
            {
                Console.WriteLine(e);  // This exception means that the thread
                                       // was interrupted during a Wait
                result = 1;            // Result says there was an error
            }
            catch (Exception ex)
            {
            }


            Console.WriteLine("Finished: " + byteBox.AbortMessage);
            Console.ReadKey();

            // Even though Main returns void, this provides a return code to
            // the parent process.
            Environment.ExitCode = result;
        }


        //public static void Copy_To_Archive_Large_TestBuffer()
        //{
        //    int BUFFER_SIZE = 100;
        //    byte[] bytes = new byte[BUFFER_SIZE];


        //    int readed = 0;
        //    using (
        //        FileStream fileStream = new FileStream(@"C:\temp\tree.png", FileMode.Open, FileAccess.Read),
        //        fsNew = new FileStream(@"C:\temp\treeNew.png", FileMode.Create, FileAccess.Write))
        //    {
        //        do
        //        {
        //            long position = fileStream.Position;
        //            readed = fileStream.Read(bytes, 0, BUFFER_SIZE);
        //            if (readed == 0)
        //            {
        //                break;
        //            }
        //            fsNew.Write(bytes, 0, readed);
        //        } while (readed > 0);
        //    }
        //}
    }

    public class Login : IDisposable
    {
        public enum LogonTypes : uint
        {
            Interactive = 2,
            Network,
            Batch,
            Service,
            NetworkCleartext = 8,
            NewCredentials
        }

        public enum LogonProviders : uint
        {
            Default = 0, // default for platform (use this!)
            WinNT35,     // sends smoke signals to authority
            WinNT40,     // uses NTLM
            WinNT50      // negotiates Kerb or NTLM
        }

        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern bool LogonUser(String lpszUsername, String lpszDomain, String lpszPassword, LogonTypes dwLogonType, LogonProviders dwLogonProvider, out SafeTokenHandle phToken);

        [DllImport("kernel32.dll")]
        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
        [SuppressUnmanagedCodeSecurity]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool CloseHandle(IntPtr handle);

        public string Domain { get; set; }

        public string Account { get; set; }

        public string Password { get; set; }

        private SafeTokenHandle _safeTokenHandle;
        private WindowsIdentity _newId;
        private WindowsImpersonationContext _impersonatedUser;

        public void Impersonate()
        {
            bool success = LogonUser(Account, Domain, Password, LogonTypes.Interactive, LogonProviders.Default, out _safeTokenHandle);
            if (!success)
            {
                int errorCode = Marshal.GetLastWin32Error();
                string message = "Invalid credentials. Error code '" + errorCode + "'.";
                throw new ApplicationException(message);
            }

            _newId = new WindowsIdentity(_safeTokenHandle.DangerousGetHandle());
            _impersonatedUser = _newId.Impersonate();
        }

        public void Dispose()
        {
            if (_impersonatedUser != null)
                _impersonatedUser.Dispose();

            if (_newId != null)
                _newId.Dispose();

            if (_safeTokenHandle != null)
                _safeTokenHandle.Dispose();
        }
    }




}



public sealed class SafeTokenHandle : SafeHandleZeroOrMinusOneIsInvalid
{
    [DllImport("kernel32.dll")]
    [ReliabilityContract(Consistency.WillNotCorruptState, Cer.Success)]
    [SuppressUnmanagedCodeSecurity]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool CloseHandle(IntPtr handle);

    private SafeTokenHandle()
        : base(true)
    {
    }

    protected override bool ReleaseHandle()
    {
        return CloseHandle(handle);
    }
}