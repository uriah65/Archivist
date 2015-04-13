using System;
using System.IO;

namespace ArchiveLib.ReaderWriter
{
    public class FileWriter
    {
        private BytesBox _box;
        private int _boxSize;
        private string _destinationFilePath;
        private Login _login;

        public FileWriter(BytesBox box, int boxSize, string destinationFilePath, Login login)
        {
            _box = box;
            _boxSize = boxSize;
            _destinationFilePath = destinationFilePath;
            _login = login;
        }

        public void ThreadRun()
        {
            byte[] bytes = new byte[_boxSize];
            FileStream fsNew = null;
            try
            {
                if (_login != null)
                {
                    // we impersonate if _login object is provided.
                    _login.Impersonate();
                }

                fsNew = new FileStream(_destinationFilePath, FileMode.Create, FileAccess.Write);

                int readCount = 0;
                do
                {
                    if (_box.AbortMessage != null)
                    {
                        break;
                    }

                    // Withdraw bytes bytes and append them to the destination file.
                    readCount = _box.WithdrawBytes(ref bytes, null);
                    fsNew.Write(bytes, 0, bytes.Length);
                } while (readCount > 0);
            }
            catch (Exception ex)
            {
                _box.WithdrawBytes(ref bytes, "Exception writing file: " + ex.Message);
            }
            finally
            {
                if (fsNew != null)
                {
                    fsNew.Dispose();
                }
                if (_login != null)
                {
                    _login.Dispose();
                }
            }
        }
    }
}