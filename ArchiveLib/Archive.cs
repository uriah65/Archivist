using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace ArchiveLib
{
    public class Archive : IDisposable, IArchive
    {
        private SafeTokenHandle _safeTokenHandle;
        private WindowsIdentity _newId;
        private WindowsImpersonationContext _impersonatedUser;

        public Archive(string accountDomain, string accountName, string accountPassword)
        {
            bool success = NativeMethods.LogonUser(accountName, accountDomain, accountPassword, NativeMethods.LogonTypes.Interactive, NativeMethods.LogonProviders.Default, out _safeTokenHandle);
            if (!success)
            {
                int errorCode = Marshal.GetLastWin32Error();
                string message = "Invalid credentials. Error code '" + errorCode + "'.";
                throw new ApplicationException(message);
            }

            _newId = new WindowsIdentity(_safeTokenHandle.DangerousGetHandle());
            _impersonatedUser = _newId.Impersonate();
        }

        #region IArchive

        public T WrapAction<T>(Func<T> action)
        {
            return action();
            //using (_safeTokenHandle)
            //{
            //    using (WindowsIdentity newId = new WindowsIdentity(_safeTokenHandle.DangerousGetHandle()))
            //    {
            //        using (WindowsImpersonationContext impersonatedUser = newId.Impersonate())
            //        {
            //            //System.Diagnostics.Debug.WriteLine("Middle of the Wrap Action: " + WindowsIdentity.GetCurrent().Name);
            //            return action();
            //        }
            //    }
            //}
        }

        public void Dispose()
        {
            if (_impersonatedUser != null)
            {
                _impersonatedUser.Dispose();
            }

            if (_newId != null)
            {
                _newId.Dispose();
            }

            if (_safeTokenHandle != null)
            {
                _safeTokenHandle.Dispose();
            }
        }

        public FileInfo GetFileInfo(string archiveFilePath)
        {
            FileInfo info = new FileInfo(archiveFilePath);
            long ln = info.Length; /* we had to get length while in secure */
            return info;

            //FileInfo info = WrapAction<FileInfo>(() =>
            //{
            //    FileInfo newInfo = new FileInfo(archiveFilePath);
            //    long ln = newInfo.Length; /* we had to get length while in secure */
            //    return newInfo;
            //});

            //return info;
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
    }
}