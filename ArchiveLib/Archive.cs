using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Principal;

namespace ArchiveLib
{
    public class Archive : IDisposable, IArchive
    {
        private string _domain;
        private string _account;
        private string _password;

        private SafeTokenHandle _safeTokenHandle;
        private WindowsIdentity _newId;
        private WindowsImpersonationContext _impersonatedUser;

        public Archive(string domain, string account, string password)
        {
            _domain = domain;
            _account = account;
            _password = password;

 

            //_newId = new WindowsIdentity(_safeTokenHandle.DangerousGetHandle());
            //_impersonatedUser = _newId.Impersonate();
        }

        #region IArchive

        public T WrapAction<T>(Func<T> action)
        {
            bool success = NativeMethods.LogonUser(_account, _domain, _password, NativeMethods.LogonTypes.Interactive, NativeMethods.LogonProviders.Default, out _safeTokenHandle);
            if (!success)
            {
                int errorCode = Marshal.GetLastWin32Error();
                string message = "Invalid credentials. Error code '" + errorCode + "'.";
                throw new ApplicationException(message);
            }

            Exception temp = null;
            using (_safeTokenHandle)
            {
                using (WindowsIdentity newId = new WindowsIdentity(_safeTokenHandle.DangerousGetHandle()))
                {
                    using (WindowsImpersonationContext impersonatedUser = newId.Impersonate())
                    {
                        try
                        {
                            //System.Diagnostics.Debug.WriteLine("Middle of the Wrap Action: " + WindowsIdentity.GetCurrent().Name);
                            return action();
                        }
                        catch (Exception ex)
                        {
                            temp = ex;
                        }
                    }
                }
            }

            if (temp != null)
            {
                throw temp;
            }

            return default(T);
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

        //[MethodImpl(MethodImplOptions.NoOptimization)]
        public FileInfo GetFileInfo(string archiveFilePath)
        {
            FileInfo info = WrapAction<FileInfo>(() =>
            {
                FileInfo newInfo = new FileInfo(archiveFilePath);
                if (newInfo.Exists)
                {
                    long ln = newInfo.Length; /* we had to get length while in secure */
                }
                return newInfo;
            });

            return info;
        }

        public void CopyToArchive(string sourceFilePath, string destinationFilePath, bool allowOverwrite = true)
        {
            byte[] buffer = null;

            MemoryStream ms = new MemoryStream();
            using (FileStream fs = File.OpenRead(sourceFilePath))
            {
                buffer = new byte[fs.Length];
                int bytesRead = 0;
                do
                {
                    bytesRead = fs.Read(buffer, 0, buffer.Length);
                    ms.Write(buffer, 0, bytesRead);
                } while (bytesRead != 0);
            }

            WrapAction<object>(() =>
            {
                if (allowOverwrite == false)
                {
                    if (File.Exists(destinationFilePath))
                    {
                        throw new ApplicationException("File already exists.");
                    }
                }

                //throw new ApplicationException("Text exception.");

                FileStream destinationFs = new FileStream(destinationFilePath, FileMode.Create);
                destinationFs.Write(buffer, 0, buffer.Length);
                destinationFs.Close();

                return null;
            });
        }

        public void CopyFromArchive(string sourceFilePath, string destinationFilePath, bool allowOverwrite = true)
        {
            if (allowOverwrite == false && File.Exists(destinationFilePath))
            {
                throw new ApplicationException("Destination file already exist.");
            }

            byte[] buffer = WrapAction<byte[]>(() =>
            {
                if (File.Exists(sourceFilePath) == false)
                {
                    throw new ApplicationException("Source file was not found.");
                }

                byte[] fileBuffer = null;
                MemoryStream ms = new MemoryStream();
                using (FileStream fs = File.OpenRead(sourceFilePath))
                {
                    fileBuffer = new byte[fs.Length];
                    int bytesRead = 0;
                    do
                    {
                        bytesRead = fs.Read(fileBuffer, 0, fileBuffer.Length);
                        ms.Write(fileBuffer, 0, bytesRead);
                    } while (bytesRead != 0);
                }
                return fileBuffer;
            });


            FileStream destinationFs = new FileStream(destinationFilePath, FileMode.Create);
            destinationFs.Write(buffer, 0, buffer.Length);
            destinationFs.Close();
        }

        public void DeleteInArchive(string destinationFilePath, bool exceptionIfNotFound = false)
        {
            WrapAction<object>(() => {
                FileInfo info = new FileInfo(destinationFilePath);
                if (!info.Exists && exceptionIfNotFound)
                {
                    throw new ApplicationException("File has not been found.");
                }

                if (info.Exists)
                {
                    File.Delete(destinationFilePath);
                }
                return null;
            });
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