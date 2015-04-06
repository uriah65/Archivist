using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Security;

namespace ArchiveLib
{
    //WindowsIdentity.Impersonate Method: https://msdn.microsoft.com/en-us/library/vstudio/w070t6ka(v=vs.110).aspx

    internal static class NativeMethods
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
    }
}

//[DllImport("kernel32.dll", CharSet = CharSet.Auto)]
//public extern static bool CloseHandle(IntPtr handle);

//[DllImport("advapi32.dll", SetLastError = true)]
//public static extern bool LogonUser(
//  string principal,
//  string authority,
//  string password,
//  LogonTypes logonType,
//  LogonProviders logonProvider,
//  out IntPtr token);

//[DllImport("kernel32.dll", SetLastError = true)]
//public static extern bool CloseHandle(IntPtr handle);