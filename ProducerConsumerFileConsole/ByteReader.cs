using System.IO;

namespace ProducerConsumerFileConsole
{
    public class FileReader
    {
        private ByteBox _box;
        private int _boxSize;
        private string _sourceFilePath;

        public FileReader(ByteBox box, int boxSize, string sourceFilePath)
        {
            _box = box;
            _boxSize = boxSize;
            _sourceFilePath = sourceFilePath;
        }

        public void ThreadRun()
        {
            byte[] bytes = new byte[_boxSize];

            using (FileStream fileStream = new FileStream(_sourceFilePath, FileMode.Open, FileAccess.Read))
            {
                int readCount = 0;
                do
                {
                    // read portion of the bytes from the source file and deposit it in the box.
                    readCount = fileStream.Read(bytes, 0, _boxSize);
                    _box.DepositBytes(bytes, readCount);
                } while (readCount > 0);
            }
        }
    }
}