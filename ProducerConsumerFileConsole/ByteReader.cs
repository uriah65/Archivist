using System;
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
            //int _exceptionCount = 2;
            FileStream fileStream = null;
            try
            {
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
            }
        }
    }
}