namespace Particular.ServiceInsight.FunctionalTests.ServiceControlStub
{
    using System.Threading.Tasks;

    public static class AsyncTask
    {
        public static Task DefaultCompleted = FromResult(default(AsyncVoid));

        public static Task<TResult> FromResult<TResult>(TResult result)
        {
            var tcs = new TaskCompletionSource<TResult>();
            tcs.SetResult(result);
            return tcs.Task;
        }

        struct AsyncVoid
        {
        }
    }
}