using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace ArchiveLib
{
    public class Archive : IDisposable
    {
        SafeTokenHandle _safeTokenHandle;

        public Archive(string accountDomain, string accountName,  string accountPassword)
        {
  
            bool loggedOn = NativeMethods.LogonUser(accountName, accountDomain, accountPassword, NativeMethods.LogonTypes.Interactive, NativeMethods.LogonProviders.Default, out _safeTokenHandle);
            if (loggedOn == false)
            {
                int errorCode = Marshal.GetLastWin32Error();
                string message = "Invalid credentials. Error code '" + errorCode + "'.";                
                throw new ApplicationException(message);
            }
 
        }

        public object WrapAction(Func<object> action)
        {
            using (_safeTokenHandle)
            {
                using (WindowsIdentity newId = new WindowsIdentity(_safeTokenHandle.DangerousGetHandle()))
                {
                    using (WindowsImpersonationContext impersonatedUser = newId.Impersonate())
                    {
                        return action();
                        //System.Diagnostics.Debug.WriteLine("After impersonation: " + WindowsIdentity.GetCurrent().Name);
                    }
                }
            }
        }

        public bool InsureDirectory(string path)
        {
            // check and create directory in the secure area
            DirectoryInfo directoryInfo = new DirectoryInfo(path);
            if (directoryInfo.Exists)
                return true;

            IntPtr token = (IntPtr) 0;
            bool loggedOn = false;// NativeMethods.LogonUser(_accountName, _accountDomain, _accountPassword, LogonTypes.Interactive, LogonProviders.Default, out token);

            if (!loggedOn)
                return false;

            WindowsIdentity identity = new WindowsIdentity(token);
            WindowsImpersonationContext context = identity.Impersonate();
            bool requireUndo = true;
            try
            {
                directoryInfo.Create();
                return true;
            }
            catch
            {
                context.Undo();
                requireUndo = false;
                throw;
            }
            finally
            {
                if (requireUndo)
                    context.Undo();
            }
        }

        public void Dispose()
        {
            //throw new NotImplementedException();
        }
    }
}
