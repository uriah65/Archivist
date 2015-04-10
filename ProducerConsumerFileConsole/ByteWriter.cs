using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProducerConsumerFileConsole
{
    public class ByteWriter
    {
        private ByteBox _cell;         // Field to hold cell object to be used
        private int _quantity = 1;  // Field for how many items to consume from cell
        private string _destinationFilePath;

        private Login _login;

        public ByteWriter(ByteBox cell, int quantity, string destinationFilePath, Login login)
        {
            _cell = cell;          // Pass in what cell object to be used
            _quantity = int.MaxValue;// quantity;  // Pass in how many items to consume from cell
            _destinationFilePath = destinationFilePath;
            _login = login;
        }

        public void ThreadRun()
        {
            _login.Impersonate();


            using (FileStream fsNew = new FileStream(_destinationFilePath, FileMode.Create, FileAccess.Write))
            {
                do
                {
                        // Consume the result by placing it in valReturned.
                    int valReturned = _cell.ReadFromCell();
                    if (valReturned == 0)
                    {
                        break;
                    }
                    fsNew.Write(_cell._bytesToRead, 0, valReturned);
                } while (true);
            }

            _login.Dispose();
        }
    }
}
