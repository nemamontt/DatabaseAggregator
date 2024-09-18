using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ViewModels
{
    public abstract class BaseInpc : INotifyPropertyChanged
    {
        protected BaseInpc() { }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void Set<T>(ref T? storage, T? newValue, [CallerMemberName] string propertyName = "")
        {
            if (!Equals(storage, newValue)) // В WPF для сравнения используется метод object.Equals(object).
            {
                T? oldValue = storage;
                storage = newValue;

                RaisePropertyChanged(propertyName);
                OnPropertyChanged(propertyName, oldValue, newValue);
            }
        }

        public static PropertyChangedEventArgs AllChanged { get; } = new(string.Empty);

        protected void RaisePropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChangedEventArgs args = AllChanged;
            if (!string.IsNullOrWhiteSpace(propertyName))
                args = new(propertyName);
            PropertyChanged?.Invoke(this, args);
        }

        protected void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            PropertyChanged?.Invoke(this, args);
        }
        protected virtual void OnPropertyChanged(string propertyName, object? oldValue, object? newValue) { }
    }
}
