namespace ServiceInsight.Tests
{
    using System.IO;
    using ApprovalTests.Core;

    class AutoApprover : IReporterWithApprovalPower
    {
        public static readonly AutoApprover INSTANCE = new AutoApprover();

        string approved;
        string received;

        public void Report(string approved, string received)
        {
            this.approved = approved;
            this.received = received;
        }

        public bool ApprovedWhenReported()
        {
            if (!File.Exists(received))
            {
                return false;
            }

            File.Delete(approved);
            if (File.Exists(approved))
            {
                return false;
            }

            File.Copy(received, approved);
            if (!File.Exists(approved))
            {
                return false;
            }

            return true;
        }
    }
}