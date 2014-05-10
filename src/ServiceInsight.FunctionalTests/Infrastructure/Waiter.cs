namespace Particular.ServiceInsight.FunctionalTests.Infrastructure
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public class Waiter
    {
        public TimeSpan UIInteractionTimeout = TimeSpan.FromSeconds(10);

        public void For(Action action, string message = null)
        {
            For(action, UIInteractionTimeout, message);
        }

        public void For(Action action, TimeSpan totalTimeout, string message = null)
        {
            var startedAt = DateTime.Now;
            bool failed;
            Exception exception = null;

            do
            {
                try
                {
                    failed = false;
                    var task = Task.Factory.StartNew(action);
                    var success = task.Wait(totalTimeout);
                    if (!success) throw new TimeoutException(message);
                }
                catch (TimeoutException)
                {
                    throw;
                }
                catch (Exception e)
                {
                    failed = true;
                    exception = e;
                    Thread.Sleep(TimeSpan.FromSeconds(1));
                }
            } while (failed && NotTimedOut(totalTimeout, startedAt));

            if (failed) throw new TimeoutException(message, exception);
        }

        private static bool NotTimedOut(TimeSpan totalTimeout, DateTime startedAt)
        {
            return DateTime.Now - startedAt < totalTimeout;
        }
    }
}