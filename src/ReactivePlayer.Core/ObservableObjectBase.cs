using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ReactivePlayer.Core
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