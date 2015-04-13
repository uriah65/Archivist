using System.Linq;
using System.Threading;

namespace ArchiveLib.ReaderWriter
{
    public class BytesBox
    {
        private bool depositIsReady;  // state flag

        public string AbortMessage { get; private set; } // exception message

        // data
        private byte[] _bytes;

        private int _bytesLength;

        public void DepositBytes(byte[] bytes, int bytesLength, string abortMessage)
        {
            lock (this)
            {
                if (abortMessage != null)
                {
                    AbortMessage = abortMessage;
                }

                if (depositIsReady)
                {
                    Monitor.Wait(this);
                }

                if (abortMessage == null)
                {
                    _bytesLength = bytesLength;
                    _bytes = bytes.ToArray();
                    //Debug.WriteLine("Deposited: {0} by {1}", _bytes.Length, WindowsIdentity.GetCurrent().Name);
                }

                depositIsReady = true;
                Monitor.Pulse(this);
            }
        }

        public int WithdrawBytes(ref byte[] bytes, string abortMessage)
        {
            lock (this)
            {
                if (abortMessage != null)
                {
                    AbortMessage = abortMessage;
                }

                if (depositIsReady == false)
                {
                    Monitor.Wait(this);
                }

                for (int i = 0; i < _bytesLength; i++)
                {
                    bytes[i] = _bytes[i];
                }

                //Debug.WriteLine("Withdrawn: {0}", _bytesLength);
                depositIsReady = false;

                Monitor.Pulse(this);
            }

            return _bytesLength;
        }
    }
}