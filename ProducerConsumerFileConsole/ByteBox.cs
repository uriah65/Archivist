using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProducerConsumerFileConsole
{
    public class ByteBox
    {
        public int BoxSize { get; private set; }

        public byte[] _bytes;
        public int _bytesInThebox;

        public byte[] _bytesToRead;
        public int _bytesInTheboxToRead;

        private int cellContents;         // Cell contents
        private bool readerFlag;  // State flag
        private int _repeatLimit;

        public ByteBox(int boxSize, int repeatLimit)
        {
            BoxSize = boxSize;
            _bytes = new byte[boxSize];
            _bytesToRead = new byte[boxSize];
            _repeatLimit = repeatLimit;
        }

        public int ReadFromCell()
        {
            lock (this)   // Enter synchronization block
            {
                if (!readerFlag)
                {            // Wait until Cell.WriteToCell is done producing
                    try
                    {
                        // Waits for the Monitor.Pulse in WriteToCell
                        Monitor.Wait(this);
                    }
                    catch (SynchronizationLockException e)
                    {
                        Console.WriteLine(e);
                    }
                    catch (ThreadInterruptedException e)
                    {
                        Console.WriteLine(e);
                    }
                }
                Console.WriteLine("Consume: {0}", cellContents);
                readerFlag = false;    // Reset the state flag to say consuming
                                       // is done.
                Monitor.Pulse(this);   // Pulse tells Cell.WriteToCell that
                                       // Cell.ReadFromCell is done.
            }   // Exit synchronization block
            return _bytesInTheboxToRead;
        }

        public void WriteToCell(int n)
        {
            lock (this)  // Enter synchronization block
            {
                if (readerFlag)
                {      // Wait until Cell.ReadFromCell is done consuming.
                    try
                    {
                        Monitor.Wait(this);   // Wait for the Monitor.Pulse in
                                              // ReadFromCell
                    }
                    catch (SynchronizationLockException e)
                    {
                        Console.WriteLine(e);
                    }
                    catch (ThreadInterruptedException e)
                    {
                        Console.WriteLine(e);
                    }
                }
                cellContents = n;

                _bytesToRead = _bytes.ToArray();
                _bytesInTheboxToRead = n;
                Console.WriteLine("Produce: {0} by {1}", cellContents, WindowsIdentity.GetCurrent().Name);
                readerFlag = true;    // Reset the state flag to say producing
                                      // is done
                Monitor.Pulse(this);  // Pulse tells Cell.ReadFromCell that
                                      // Cell.WriteToCell is done.
            }   // Exit synchronization block
        }
    }
}
