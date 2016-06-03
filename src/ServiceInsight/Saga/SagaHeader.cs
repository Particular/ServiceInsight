namespace ServiceInsight.Saga
{
    public class SagaHeader
    {
        readonly SagaData data;

        public SagaHeader(SagaData data)
        {
            this.data = data;
        }

        public SagaData Data => data;
    }
}