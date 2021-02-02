namespace ServiceInsight.Saga
{
    public class SagaFooter
    {
        public SagaFooter(SagaData data)
        {
            Data = data;
        }

        public SagaData Data { get; }

        public bool IsCompleted => Data?.IsCompleted ?? true;
    }
}