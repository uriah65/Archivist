using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace ArchiveLib
{
    public class Archive : IDisposable, IArchive
    {
        private SafeTokenHandle _safeTokenHandle;

        public Archive(string accountDomain, string accountName, string accountPassword)
        {
            bool loggedOn = NativeMethods.LogonUser(accountName, accountDomain, accountPassword, NativeMethods.LogonTypes.Interactive, NativeMethods.LogonProviders.Default, out _safeTokenHandle);
            if (loggedOn == false)
            {
                int errorCode = Marshal.GetLastWin32Error();
                string message = "Invalid credentials. Error code '" + errorCode + "'.";
                throw new ApplicationException(message);
            }
        }

        #region IArchive

        public T WrapAction<T>(Func<T> action)
        {
            using (_safeTokenHandle)
            {
                using (WindowsIdentity newId = new WindowsIdentity(_safeTokenHandle.DangerousGetHandle()))
                {
                    using (WindowsImpersonationContext impersonatedUser = newId.Impersonate())
                    {
                        //System.Diagnostics.Debug.WriteLine("After impersonation: " + WindowsIdentity.GetCurrent().Name);
                        return action();                        
                    }
                }
            }
        }

        public FileInfo GetFileInfo(string archiveFilePath)
        {
            FileInfo info = WrapAction<FileInfo>(() => {
                FileInfo newInfo = new FileInfo(archiveFilePath);
                return newInfo; });

            return info;
        }

        #endregion IArchive

        public bool InsureDirectory(string path)
        {
            // check and create directory in the secure area
            DirectoryInfo directoryInfo = new DirectoryInfo(path);
            if (directoryInfo.Exists)
                return true;

            IntPtr token = (IntPtr)0;
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