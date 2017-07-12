using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ReactivePlayer.App
{
    public abstract class ObservableObjectBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void SetAndRaiseIfChanged<T>(
            ref T backingField,
            T newValue,
            [CallerMemberName] string propertyName = null)
        {
            // TODO: return newValue/null/bool/void?

            if (!object.Equals(backingField, newValue))
                return;

            backingField = newValue;
            this.RaisePropertyChanged(propertyName);
        }
    }
}