namespace Particular.ServiceInsight.Tests.Helpers
{
    using System.Threading.Tasks;

    public static class NSubstituteHelper
    {
        public static void IgnoreAwait(this Task task)
        {
        }
    }
}