using System;
using System.IO;

namespace ProducerConsumerFileConsole
{
    public class FileWriter
    {
        private BytesBox _box;
        private string _destinationFilePath;
        private Login _login;

        public FileWriter(BytesBox box, string destinationFilePath, Login login)
        {
            _box = box;
            _destinationFilePath = destinationFilePath;
            _login = login;
        }

        public void ThreadRun()
        {
            //int exceptionCount = 2;
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
                    //if (--exceptionCount == 0) throw new ApplicationException("Test exception in Writer");
                    if (_box.AbortMessage != null)
                    {
                        break;
                    }

                    // Withdraw bytes bytes and append them to the destination file.
                    byte[] bytes = _box.WithdrawBytes(null);
                    readCount = bytes.Length;
                    fsNew.Write(bytes, 0, bytes.Length);
                } while (readCount > 0);
            }
            catch (Exception ex)
            {
                _box.WithdrawBytes("Exception writing file: " + ex.Message);
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