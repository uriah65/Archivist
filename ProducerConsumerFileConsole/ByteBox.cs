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
        public int _boxSize; 

        public byte[] _bytes;
        public int _bytesInThebox;


        private int cellContents;         // Cell contents
        private bool readerFlag;  // State flag
        private int _repeatLimit;

        public ByteBox(int boxSize, int repeatLimit)
        {
            _boxSize = boxSize;
            _bytes = new byte[boxSize];
            _repeatLimit = repeatLimit;
        }

        public byte[] ReadFromCell()
        {
            byte[] result = null;

            lock (this)   // Enter synchronization block
            {
                if (readerFlag == false)
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

                //result = 
                result = new byte[_bytesInThebox];
                for (int i = 0; i < _bytesInThebox; i++)
                {
                    result[i] = _bytes[i];
                }

                Console.WriteLine("Consume: {0}", cellContents);
                readerFlag = false;    // Reset the state flag to say consuming
                                       // is done.
                Monitor.Pulse(this);   // Pulse tells Cell.WriteToCell that
                                       // Cell.ReadFromCell is done.
            }   // Exit synchronization block

            return result;
        }

        public void WriteToCell(byte[] bytes, int realSize)
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


                //_bytes = bytes.ToArray();
                //if (_bytes == bytes)
                //{
                //}
                //else
                //{
                //}

                cellContents = realSize;
                _bytesInThebox = realSize;
                _bytes = bytes.ToArray();
                //for (int i = 0; i < bytes.Length; i++)
                //{
                //    _bytes[i] = bytes[i];
                //}

                Console.WriteLine("Produce: {0} by {1}", cellContents, WindowsIdentity.GetCurrent().Name);
                readerFlag = true;    // Reset the state flag to say producing
                                      // is done
                Monitor.Pulse(this);  // Pulse tells Cell.ReadFromCell that
                                      // Cell.WriteToCell is done.
            }   // Exit synchronization block
        }
    }
}
