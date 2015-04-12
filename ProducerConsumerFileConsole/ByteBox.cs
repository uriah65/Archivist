using System;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using System.Threading;

namespace ProducerConsumerFileConsole
{
    public class ByteBox
    {
        private bool readerFlag;  // State flag

        public byte[] _bytes;
        public int _bytesInThebox;

        public ByteBox()
        {
        }

        public void DepositBytes(byte[] bytes, int realSize)
        {
            lock (this)
            {
                if (readerFlag)
                {
                    try
                    {
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

                _bytesInThebox = realSize;
                _bytes = bytes.ToArray();

                Debug.WriteLine("Produce: {0} by {1}", _bytes.Length, WindowsIdentity.GetCurrent().Name);

                readerFlag = true;
                Monitor.Pulse(this);
            }   // Exit synchronization block
        }

        public byte[] WithdrawBytes()
        {
            byte[] result = null;

            lock (this)
            {
                if (readerFlag == false)
                {
                    try
                    {
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

                result = new byte[_bytesInThebox];
                for (int i = 0; i < _bytesInThebox; i++)
                {
                    result[i] = _bytes[i];
                }

                Debug.WriteLine("Consume: {0}", result.Length);
                readerFlag = false;

                Monitor.Pulse(this);
            }   // Exit synchronization block

            return result;
        }
    }
}