using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProducerConsumerFileConsole
{
    public class ByteReader
    {
        private ByteBox _cell;         // Field to hold cell object to be used
        private int _quantity;      // Field for how many items to produce in cell
        private string _sourceFilePath;

        public ByteReader(ByteBox cell, int quantity, string sourceFilePath)
        {
            _cell = cell;          // Pass in what cell object to be used
            _quantity = quantity;  // Pass in how many items to produce in cell
            _sourceFilePath = sourceFilePath;
        }

        public void ThreadRun()
        {
            //for (int i = 1; i <= _quantity; i++)
            //{
            //    _cell._bytes[0] = (byte)(i + 1);
            //    _cell._bytesInThebox = 1;
            //    _cell.WriteToCell(i);  // "producing"
            //}

            using (FileStream fileStream = new FileStream(@"C:\temp\tree.png", FileMode.Open, FileAccess.Read))
            {
                do
                {
                    int readed = fileStream.Read(_cell._bytes, 0, _cell._bytes.Length);
                    _cell.WriteToCell(readed);
                    if (readed == 0)
                    {
                        break;
                    }
                } while (true);
            }

        }
    }
}
