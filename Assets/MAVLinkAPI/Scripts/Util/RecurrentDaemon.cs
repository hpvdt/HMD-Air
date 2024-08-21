using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace MAVLinkAPI.Scripts.Util
{
    public abstract class Daemon : IDisposable
    {
        // Can only be canceled once, as a result, it can only be started once
        public CancellationTokenSource Cancel = new();

        private readonly CancellationToken _cancelSignal;

        public Daemon()
        {
            _cancelSignal = Cancel.Token;
        }

        ~Daemon()
        {
            Dispose();
        }

        public void Dispose()
        {
            Cancel.CancelAfter(5000);
        }

        public async void Start()
        {
            await Task.Run(() =>
                {
                    try
                    {
                        Execute(_cancelSignal);
                    }
                    catch (Exception e)
                    {
                        Debug.LogException(e);
                    }
                }, _cancelSignal // hard cancel after 5 seconds
            );
        }

        public abstract void Execute(CancellationToken cancelSignal);
    }

    // once created, will repeatedly do something
    public abstract class RecurrentDaemon : Daemon
    {
        public readonly AtomicLong Counter = new();

        public override void Execute(CancellationToken cancelSignal)
        {
            while (!cancelSignal.IsCancellationRequested) // soft cancel immediately
            {
                Counter.Increment();
                Iterate();
            }
        }

        protected abstract void Iterate();
    }
}