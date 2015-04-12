using System;
using System.Diagnostics;
using System.Linq;
using System.Security.Principal;
using System.Threading;

namespace ProducerConsumerFileConsole
{
    public class ByteBox
    {
        private bool readerFlag;  // state flag
        public string AbortMessage { get; private set; } // exception message

        // data
        private byte[] _bytes;
        private int _bytesInThebox;

        public void DepositBytes(byte[] bytes, int realSize, string abortMessage)
        {
            lock (this)
            {
                if (abortMessage != null)
                {
                    AbortMessage = abortMessage;
                }

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

                if (abortMessage == null)
                {
                    _bytesInThebox = realSize;
                    _bytes = bytes.ToArray();

                    Debug.WriteLine("Produce: {0} by {1}", _bytes.Length, WindowsIdentity.GetCurrent().Name);
                }

                readerFlag = true;
                Monitor.Pulse(this);
            }   // Exit synchronization block
        }

        public byte[] WithdrawBytes(string abortMessage)
        {
            byte[] result = null;

            lock (this)
            {
                if (abortMessage != null)
                {
                    AbortMessage = abortMessage;
                }

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