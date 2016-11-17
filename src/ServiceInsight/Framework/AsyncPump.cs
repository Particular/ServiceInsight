﻿namespace ServiceInsight.Framework
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows.Threading;

    static class AsyncPump
    {
        public static void Run(Func<Task> func)
        {
            if (func == null)
            {
                throw new ArgumentNullException(nameof(func));
            }

            var prevCtx = SynchronizationContext.Current;
            try
            {
                var syncCtx = new DispatcherSynchronizationContext();
                SynchronizationContext.SetSynchronizationContext(syncCtx);

                var t = func();
                if (t == null)
                {
                    throw new InvalidOperationException();
                }

                var frame = new DispatcherFrame();
                t.ContinueWith(_ => { frame.Continue = false; },
                    TaskScheduler.Default);
                Dispatcher.PushFrame(frame);

                t.GetAwaiter().GetResult();
            }
            finally
            {
                SynchronizationContext.SetSynchronizationContext(prevCtx);
            }
        }

        public static T Run<T>(Func<Task<T>> func)
        {
            if (func == null)
            {
                throw new ArgumentNullException(nameof(func));
            }

            var prevCtx = SynchronizationContext.Current;
            try
            {
                var syncCtx = new DispatcherSynchronizationContext();
                SynchronizationContext.SetSynchronizationContext(syncCtx);

                var t = func();
                if (t == null)
                {
                    throw new InvalidOperationException();
                }

                var frame = new DispatcherFrame();
                t.ContinueWith(_ => { frame.Continue = false; },
                    TaskScheduler.Default);
                Dispatcher.PushFrame(frame);

                return t.GetAwaiter().GetResult();
            }
            finally
            {
                SynchronizationContext.SetSynchronizationContext(prevCtx);
            }
        }
    }
}