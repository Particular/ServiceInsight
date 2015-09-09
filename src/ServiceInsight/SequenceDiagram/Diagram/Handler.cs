namespace Particular.ServiceInsight.Desktop.SequenceDiagram.Diagram
{
    using System;

    public class Handler : DiagramItem
    {
        public Endpoint Endpoint
        {
            get { return _endpoint; }
            set
            {
                _endpoint = value;
                OnPropertyChanged("Endpoint");
            }
        }

        public override double X
        {
            get { return Endpoint.X; }
            set
            {
            }
        }

        private double y;
        public override double Y
        {
            get { return y; }
            set
            {
                //rounds the value to be multiples of 50.
                y = (Math.Round(value / 50.0)) * 50;
                OnPropertyChanged("Y");
            }
        }

        private bool _isHighlighted;
        private Endpoint _endpoint;

        public bool IsHighlighted
        {
            get
            {
                return _isHighlighted;
            }
            set
            {
                _isHighlighted = value;
                OnPropertyChanged("IsHighlighted");
            }
        }

    }
}