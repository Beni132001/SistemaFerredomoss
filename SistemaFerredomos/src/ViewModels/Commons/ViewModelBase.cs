using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SistemaFerredomos.src.ViewModels.Commons
{
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private bool _isUpdatingProperty;
        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
        {
            if (_isUpdatingProperty || EqualityComparer<T>.Default.Equals(field, value))
                return false;

            _isUpdatingProperty = true;
            field = value;
            OnPropertyChanged(propertyName);
            _isUpdatingProperty = false;
            return true;
        }
    }
}