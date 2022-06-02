namespace WpfApp1.ViewModels
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Runtime.CompilerServices;
    using Annotations;

    public class MainViewModel : INotifyPropertyChanged
    {
        private IList<Person> _persons;
        private SelectedRowContext _selection;

        public MainViewModel()
        {
            Persons = new List<Person>();
            Selection = new SelectedRowContext();

            for (int i = 0; i < 200; i++)
            {
                Persons.Add(new Person{Name=Guid.NewGuid().ToString()});
            }
        }

        public SelectedRowContext Selection
        {
            get => _selection;
            set
            {
                _selection = value;
                OnPropertyChanged();
            }
        }

        public IList<Person> Persons
        {
            get => _persons;
            set
            {
               _persons = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
