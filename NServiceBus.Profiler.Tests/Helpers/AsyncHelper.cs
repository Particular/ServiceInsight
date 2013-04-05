using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NServiceBus.Profiler.Tests.Helpers
{
    public static class AsyncHelper
    {
        public static void Run(Action asyncMethod)
        {
            if (asyncMethod == null) 
                throw new ArgumentNullException("asyncMethod");

            var prevCtx = SynchronizationContext.Current;
            try
            {
                var syncCtx = new SingleThreadSynchronizationContext(true);
                SynchronizationContext.SetSynchronizationContext(syncCtx);

                syncCtx.OperationStarted();
                asyncMethod();
                syncCtx.OperationCompleted();

                syncCtx.RunOnCurrentThread();
            }
            finally
            {
                SynchronizationContext.SetSynchronizationContext(prevCtx);
            }
        }

        public static void Run(Func<Task> asyncMethod)
        {
            if (asyncMethod == null) 
                throw new ArgumentNullException("asyncMethod");

            var prevCtx = SynchronizationContext.Current;
            try
            {
                var syncCtx = new SingleThreadSynchronizationContext(false);
                SynchronizationContext.SetSynchronizationContext(syncCtx);

                var t = asyncMethod();

                if (t == null)
                    throw new InvalidOperationException("No task provided.");

                t.ContinueWith(delegate { syncCtx.Complete(); }, TaskScheduler.Default);

                syncCtx.RunOnCurrentThread();
                t.GetAwaiter().GetResult();
            }
            finally
            {
                SynchronizationContext.SetSynchronizationContext(prevCtx);
            }
        }

        public static T Run<T>(Func<Task<T>> asyncMethod)
        {
            if (asyncMethod == null) 
                throw new ArgumentNullException("asyncMethod");

            var prevCtx = SynchronizationContext.Current;
            try
            {
                var syncCtx = new SingleThreadSynchronizationContext(false);
                SynchronizationContext.SetSynchronizationContext(syncCtx);

                var t = asyncMethod();
                if (t == null)
                    throw new InvalidOperationException("No task provided.");

                t.ContinueWith(delegate { syncCtx.Complete(); }, TaskScheduler.Default);

                syncCtx.RunOnCurrentThread();
                return t.GetAwaiter().GetResult();
            }
            finally
            {
                SynchronizationContext.SetSynchronizationContext(prevCtx);
            }
        }

        private sealed class SingleThreadSynchronizationContext : SynchronizationContext
        {
            private readonly BlockingCollection<KeyValuePair<SendOrPostCallback, object>> _queue;
            private int _operationCount;
            private readonly bool _trackOperations;

            internal SingleThreadSynchronizationContext(bool trackOperations)
            {
                _queue = new BlockingCollection<KeyValuePair<SendOrPostCallback, object>>();
                _trackOperations = trackOperations;
            }

            /// <summary>Dispatches an asynchronous message to the synchronization context.</summary>
            /// <param name="d">The System.Threading.SendOrPostCallback delegate to call.</param>
            /// <param name="state">The object passed to the delegate.</param>
            public override void Post(SendOrPostCallback d, object state)
            {
                if (d == null) 
                    throw new ArgumentNullException("d");

                _queue.Add(new KeyValuePair<SendOrPostCallback, object>(d, state));
            }

            public override void Send(SendOrPostCallback d, object state)
            {
                throw new NotSupportedException("Synchronously sending is not supported.");
            }

            public void RunOnCurrentThread()
            {
                foreach (var workItem in _queue.GetConsumingEnumerable())
                    workItem.Key(workItem.Value);
            }

            public void Complete()
            {
                _queue.CompleteAdding();
            }

            public override void OperationStarted()
            {
                if (_trackOperations)
                    Interlocked.Increment(ref _operationCount);
            }

            public override void OperationCompleted()
            {
                if (_trackOperations && Interlocked.Decrement(ref _operationCount) == 0)
                    Complete();
            }
        }
    }
}