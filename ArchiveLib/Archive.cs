using ArchiveLib.ReaderWriter;
using System;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Runtime.InteropServices;
using System.Security.Principal;
using System.Threading;

namespace ArchiveLib
{
    public class Archive : /*IDisposable,*/ IArchive
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

            CopyWithThreads(sourceFilePath, destinationFilePath, true, allowOverwrite);


            //byte[] buffer = null;

            //MemoryStream ms = new MemoryStream();
            //using (FileStream fs = File.OpenRead(sourceFilePath))
            //{
            //    buffer = new byte[fs.Length];
            //    int bytesRead = 0;
            //    do
            //    {
            //        bytesRead = fs.Read(buffer, 0, buffer.Length);
            //        ms.Write(buffer, 0, bytesRead);
            //    } while (bytesRead != 0);
            //}

            //WrapAction<object>(() =>
            //{
            //    if (allowOverwrite == false)
            //    {
            //        if (File.Exists(destinationFilePath))
            //        {
            //            throw new ApplicationException("File already exists.");
            //        }
            //    }

            //    //throw new ApplicationException("Text exception.");

            //    FileStream destinationFs = new FileStream(destinationFilePath, FileMode.Create);
            //    destinationFs.Write(buffer, 0, buffer.Length);
            //    destinationFs.Close();

            //    return null;
            //});
        }

        public void CopyFromArchive(string sourceFilePath, string destinationFilePath, bool allowOverwrite = true)
        {
            CopyWithThreads(sourceFilePath, destinationFilePath, false, allowOverwrite);

            //if (allowOverwrite == false && File.Exists(destinationFilePath))
            //{
            //    throw new ApplicationException("Destination file already exist.");
            //}

            //byte[] buffer = WrapAction<byte[]>(() =>
            //{
            //    if (File.Exists(sourceFilePath) == false)
            //    {
            //        throw new ApplicationException("Source file was not found.");
            //    }

            //    byte[] fileBuffer = null;
            //    MemoryStream ms = new MemoryStream();
            //    using (FileStream fs = File.OpenRead(sourceFilePath))
            //    {
            //        fileBuffer = new byte[fs.Length];
            //        int bytesRead = 0;
            //        do
            //        {
            //            bytesRead = fs.Read(fileBuffer, 0, fileBuffer.Length);
            //            ms.Write(fileBuffer, 0, bytesRead);
            //        } while (bytesRead != 0);
            //    }
            //    return fileBuffer;
            //});

            //FileStream destinationFs = new FileStream(destinationFilePath, FileMode.Create);
            //destinationFs.Write(buffer, 0, buffer.Length);
            //destinationFs.Close();
        }

        public void CopyWithingArchive(string sourceFilePath, string destinationFilePath, bool allowOverwrite = true, bool deleteSource = false)
        {
            WrapAction<object>(() =>
            {
                File.Copy(sourceFilePath, destinationFilePath, allowOverwrite);

                if (deleteSource)
                {
                    File.Delete(sourceFilePath);
                }
                return null;
            });
        }

        public void DeleteInArchive(string destinationFilePath, bool exceptionIfNotFound = false)
        {
            WrapAction<object>(() =>
            {
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

        public void InsureArchiveDirectory(string destinationDirectory)
        {
            WrapAction<object>(() =>
            {
                DirectoryInfo info = new DirectoryInfo(destinationDirectory);
                if (!info.Exists)
                {
                    info.Create();
                }
                return null;
            });
        }

        public void CopyToArchive_MemoryFile(string sourceFilePath, string destinationFilePath, bool allowOverwrite = true)
        {
            byte[] buffer = null;

            long blockSize = 10000000;
            long ln = (new FileInfo(sourceFilePath)).Length;

            using (var mmf = MemoryMappedFile.CreateFromFile(sourceFilePath, FileMode.Open, "Temp"))
            {
                WrapAction<object>(() =>
                {
                    if (allowOverwrite == false)
                    {
                        if (File.Exists(destinationFilePath))
                        {
                            throw new ApplicationException("File already exists.");
                        }
                    }

                    FileStream destinationFs = new FileStream(destinationFilePath, FileMode.Create);

                    long offset = 0;
                    long length = 0;
                    long i = 0;
                    do
                    {
                        offset = i * blockSize;
                        length = blockSize;
                        if (offset + length > ln)
                        {
                            length = ln - offset;
                        }

                        using (var reader = mmf.CreateViewAccessor(offset, length, MemoryMappedFileAccess.Read))
                        {
                            System.Diagnostics.Debug.WriteLine("iteration  i=" + i + " offset=" + offset + " length=" + length);

                            buffer = new byte[length];
                            reader.ReadArray<byte>(0, buffer, 0, (int)length);

                            destinationFs.Write(buffer, 0, buffer.Length);
                            destinationFs.Flush(true);
                        }
                        i++;
                    } while (offset + length < ln);

                    destinationFs.Close();

                    return null;
                });
            }
        }

        public void CopyWithThreads(string sourceFilePath, string destinationFilePath, bool toArchive, bool allowOverwrite = true)
        {
            Login login = new Login();
            login.Domain = _domain;
            login.Account = _account;
            login.Password = _password;

            //int boxSize = (int)Math.Pow(2, 20); //1000000; // 1MB
            int boxSize = 10000000;

            BytesBox byteBox = new BytesBox();

            FileReader reader = new FileReader(byteBox, boxSize, sourceFilePath, toArchive? null : login);
            FileWriter writer = new FileWriter(byteBox, boxSize, destinationFilePath, toArchive? login : null);

            Thread readerThread = new Thread(new ThreadStart(reader.ThreadRun));
            Thread writerThread = new Thread(new ThreadStart(writer.ThreadRun));

            readerThread.Start();
            writerThread.Start();

            readerThread.Join();
            writerThread.Join();
        }

        #endregion IArchive
    }
}