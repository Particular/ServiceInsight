namespace Particular.ServiceInsight.Tests.Helpers
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public class AsyncHelper
    {
        public static void Run(Action asyncMethod)
        {
            if (asyncMethod == null) 
                throw new ArgumentNullException("asyncMethod");

            var previousContext = SynchronizationContext.Current;
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
                SynchronizationContext.SetSynchronizationContext(previousContext);
            }
        }

        public static void Run(Func<Task> asyncMethod)
        {
            if (asyncMethod == null) 
                throw new ArgumentNullException("asyncMethod");

            var previousContext = SynchronizationContext.Current;
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
                SynchronizationContext.SetSynchronizationContext(previousContext);
            }
        }

        public static T Run<T>(Func<Task<T>> asyncMethod)
        {
            if (asyncMethod == null) 
                throw new ArgumentNullException("asyncMethod");

            var previousContext = SynchronizationContext.Current;
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
                SynchronizationContext.SetSynchronizationContext(previousContext);
            }
        }

        sealed class SingleThreadSynchronizationContext : SynchronizationContext
        {
            BlockingCollection<KeyValuePair<SendOrPostCallback, object>> queue;
            int operationCount;
            bool trackOperations;

            internal SingleThreadSynchronizationContext(bool trackOperations)
            {
                queue = new BlockingCollection<KeyValuePair<SendOrPostCallback, object>>();
                this.trackOperations = trackOperations;
            }

            /// <summary>Dispatches an asynchronous message to the synchronization context.</summary>
            /// <param name="d">The System.Threading.SendOrPostCallback delegate to call.</param>
            /// <param name="state">The object passed to the delegate.</param>
            public override void Post(SendOrPostCallback d, object state)
            {
                if (d == null) 
                    throw new ArgumentNullException("d");

                queue.Add(new KeyValuePair<SendOrPostCallback, object>(d, state));
            }

            public override void Send(SendOrPostCallback d, object state)
            {
                throw new NotSupportedException("Synchronously sending is not supported.");
            }

            public void RunOnCurrentThread()
            {
                foreach (var workItem in queue.GetConsumingEnumerable())
                    workItem.Key(workItem.Value);
            }

            public void Complete()
            {
                queue.CompleteAdding();
            }

            public override void OperationStarted()
            {
                if (trackOperations)
                    Interlocked.Increment(ref operationCount);
            }

            public override void OperationCompleted()
            {
                if (trackOperations && Interlocked.Decrement(ref operationCount) == 0)
                    Complete();
            }
        }
    }
}