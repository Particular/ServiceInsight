namespace Particular.ServiceInsight.Desktop.Saga
{
    public class SagaFooter
    {
        private readonly SagaData data;

        public SagaFooter(SagaData data)
        {
            this.data = data;
        }

        public SagaData Data
        {
            get { return data; }
        }

        public bool IsCompleted
        {
            get { return data.IsCompleted; }
        }
    }
}