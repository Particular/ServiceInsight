namespace Particular.ServiceInsight.Desktop.Saga
{
    public class SagaHeader
    {
        private readonly SagaData data;

        public SagaHeader(SagaData data)
        {
            this.data = data;
        }

        public SagaData Data
        {
            get { return data; }
        }
    }
}