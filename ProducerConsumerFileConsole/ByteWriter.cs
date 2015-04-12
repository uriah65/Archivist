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
            _login.Impersonate();

            using (FileStream fsNew = new FileStream(_destinationFilePath, FileMode.Create, FileAccess.Write))
            {
                int readCount = 0;
                do
                {
                    // Withdraw bytes bytes and append them to the destination file.
                    byte[] bytes = _box.WithdrawBytes();
                    readCount = bytes.Length;
                    fsNew.Write(bytes, 0, bytes.Length);
                } while (readCount > 0);
            }

            _login.Dispose();
        }
    }
}