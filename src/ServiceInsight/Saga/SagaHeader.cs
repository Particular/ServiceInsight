namespace ServiceInsight.Saga
{
    public class SagaHeader
    {
        public SagaHeader(SagaData data)
        {
            Data = data;
        }

        public SagaData Data { get; }
    }
}