namespace ServiceInsight.ServiceControl
{
    public class PresentationBody
    {
        PresentationHint hint = PresentationHint.Standard;

        public string Text { get; set; }

        public PresentationHint Hint
        {
            get { return hint; }
            set { hint = value; }
        }
    }
}
