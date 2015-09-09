namespace Particular.ServiceInsight.Desktop.SequenceDiagram.Diagram
{
    using System.ComponentModel;

    public abstract class DiagramItem : INotifyPropertyChanged
    {
        private string name;
        private double x;
        private double y;

        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                OnPropertyChanged("Name");
            }
        }

        public virtual double X
        {
            get { return x; }
            set
            {
                x = value;
                OnLocationChanged();
                OnPropertyChanged("X");
            }
        }

        public virtual double Y
        {
            get { return y; }
            set
            {
                y = value;
                OnLocationChanged();
                OnPropertyChanged("Y");
            }
        }

        protected virtual void OnLocationChanged()
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}