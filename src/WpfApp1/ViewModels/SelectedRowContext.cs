namespace WpfApp1.ViewModels
{
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using Annotations;

    public class SelectedRowContext : INotifyPropertyChanged
    {
        private Person _selectedPerson;

        public Person SelectedPerson
        {
            get => _selectedPerson;
            set
            {
                _selectedPerson = value;
                OnPropertyChanged();
                OnPersonSelected();
            }
        }

        private void OnPersonSelected()
        {
            Trace.WriteLine("Selected Person: " + (SelectedPerson == null ? "<null>" : SelectedPerson.Name));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}