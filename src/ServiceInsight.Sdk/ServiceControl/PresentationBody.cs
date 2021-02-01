namespace ServiceInsight.ServiceControl
{
    public class PresentationBody
    {
        public string Text { get; set; }

        public PresentationHint Hint { get; set; } = PresentationHint.Standard;
    }
}
