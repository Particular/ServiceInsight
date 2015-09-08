namespace ServiceInsight.SequenceDiagram.Drawing
{
    using System;
    using System.Collections.Generic;

    public class HandlerViewModel : UmlViewModel
    {
        public HandlerViewModel()
        {
            Out = new List<ArrowViewModel>();
        }

        public bool IsPartOfSaga()
        {
            return string.IsNullOrWhiteSpace(PartOfSaga);
        }

        public HandlerStateType State { get; set; }
        public string PartOfSaga { get; set; }
        public ArrowViewModel In { get; set; }
        public List<ArrowViewModel> Out { get; set; }
        public DateTime? HandledAt { get; set; }
    }
}