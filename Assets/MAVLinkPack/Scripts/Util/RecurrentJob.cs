using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace MAVLinkPack.Scripts.Util
{
    public abstract class Daemon : IDisposable
    {
        public CancellationTokenSource Cancel = new();

        ~Daemon()
        {
            Dispose();
        }

        public void Dispose()
        {
            Stop();
        }

        public void Stop()
        {
            Cancel.CancelAfter(5000);
        }
    }

    // once created, will repeatedly do something
    public abstract class RecurrentJob : Daemon
    {
        public readonly AtomicLong Counter = new();

        public async void Start()
        {
            var cancelSignal = Cancel.Token;

            await Task.Run(() =>
                {
                    while (!cancelSignal.IsCancellationRequested) // soft cancel immediately
                        try
                        {
                            Counter.Increment();
                            Iterate();
                        }
                        catch (Exception e)
                        {
                            Debug.LogException(e);
                        }
                }, cancelSignal // hard cancel after 5 seconds
            );
        }

        protected abstract void Iterate();
    }
}