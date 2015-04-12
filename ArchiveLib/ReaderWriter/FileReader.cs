using System;
using System.IO;

namespace ArchiveLib.ReaderWriter
{
    public class FileReader
    {
        private BytesBox _box;
        private int _boxSize;
        private string _sourceFilePath;
        private Login _login;

        public FileReader(BytesBox box, int boxSize, string sourceFilePath, Login login)
        {
            _box = box;
            _boxSize = boxSize;
            _sourceFilePath = sourceFilePath;
            _login = login;
        }

        public void ThreadRun()
        {
            //int _exceptionCount = 2;
            FileStream fileStream = null;
            try
            {
                if (_login != null)
                {
                    // we impersonate if _login object is provided.
                    _login.Impersonate();
                }

                byte[] bytes = new byte[_boxSize];

                fileStream = new FileStream(_sourceFilePath, FileMode.Open, FileAccess.Read);

                int readCount = 0;
                do
                {
                    if (_box.AbortMessage != null)
                    {
                        break;
                    }

                    //if (--_exceptionCount == 0) throw new ApplicationException("Test exception in reader");

                    // read portion of the bytes from the source file and deposit it in the box.
                    readCount = fileStream.Read(bytes, 0, _boxSize);
                    _box.DepositBytes(bytes, readCount, null);
                } while (readCount > 0);
            }
            catch (Exception ex)
            {
                _box.DepositBytes(null, 0, "Exception reading file: " + ex.Message);
            }
            finally
            {
                if (fileStream != null)
                {
                    fileStream.Dispose();
                }
                if (_login != null)
                {
                    _login.Dispose();
                }
            }
        }
    }
}