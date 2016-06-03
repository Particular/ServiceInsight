namespace ServiceInsight.Saga
{
    public class SagaFooter
    {
        readonly SagaData data;

        public SagaFooter(SagaData data)
        {
            this.data = data;
        }

        public SagaData Data => data;

        public bool IsCompleted => data != null ? data.IsCompleted : true;
    }
}