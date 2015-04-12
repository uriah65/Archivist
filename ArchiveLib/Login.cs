using System;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace ArchiveLib
{
    public class Login : IDisposable
    {
        public string Domain { get; set; }

        public string Account { get; set; }

        public string Password { get; set; }

        private SafeTokenHandle _safeTokenHandle;
        private WindowsIdentity _newId;
        private WindowsImpersonationContext _impersonatedUser;

        public void Impersonate()
        {
            bool success = NativeMethods.LogonUser(Account, Domain, Password, NativeMethods.LogonTypes.Interactive, NativeMethods.LogonProviders.Default, out _safeTokenHandle);
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