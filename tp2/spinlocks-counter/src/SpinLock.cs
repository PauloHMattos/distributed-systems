using System.Threading;

namespace tp2
{
    public class SpinLock
    {
        private int _locked;

        public SpinLock()
        {
            _locked = 0;
        }

        public void Acquire()
        {
            while (Interlocked.CompareExchange(ref _locked, 1, 0) == 1)
            {
            }
        }

        public void Release()
        {
            _locked = 0;
        }
    }
}
