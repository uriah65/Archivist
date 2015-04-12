using System;
using System.IO;

namespace ProducerConsumerFileConsole
{
    public class FileWriter
    {
        private ByteBox _box;
        private string _destinationFilePath;
        private Login _login;

        public FileWriter(ByteBox box, string destinationFilePath, Login login)
        {
            _box = box;
            _destinationFilePath = destinationFilePath;
            _login = login;
        }

        public void ThreadRun()
        {
            int exceptionCount = -2;
            try
            {
                _login.Impersonate();

                using (FileStream fsNew = new FileStream(_destinationFilePath, FileMode.Create, FileAccess.Write))
                {
                    int readCount = 0;
                    do
                    {
                        if (--exceptionCount == 0) throw new ApplicationException("Test exception in Writer");
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
            }
            catch (Exception ex)
            {
                _box.WithdrawBytes(ex.Message);
            }
            finally
            {
                _login.Dispose();
            }
        }
    }
}