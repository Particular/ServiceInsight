using System.Threading.Tasks;

namespace NServiceBus.Profiler.Tests.Helpers
{
    public static class NSubstituteHelper
    {
        public static void IgnoreAwait(this Task task)
        {
        }
    }
}