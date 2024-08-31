using System;
using System.Threading;
using System.Threading.Tasks;

namespace MAVLinkKit.Scripts.Util
{
    // once created, will repeatedly do something
    public abstract class RecurrentJob : IDisposable
    {
        public CancellationTokenSource Cancel = new CancellationTokenSource();

        ~RecurrentJob()
        {
            Dispose();
        }

        public void Dispose()
        {
            Stop();
        }

        public async void Start()
        {
            var shouldStop = Cancel.Token;

            await Task.Run(() =>
                {
                    while (!shouldStop.IsCancellationRequested) // soft cancel immediately
                    {
                        Do();
                    }
                }, shouldStop // hard cancel after 5 seconds
            );
        }

        public void Stop()
        {
            Cancel.CancelAfter(5000);
        }

        public abstract void Do();
    }
}