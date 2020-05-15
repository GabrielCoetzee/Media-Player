using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Generic.Property_Notify
{
    public class PropertyNotifyBase : INotifyPropertyChanged
    {
        #region Interface Implementations

        public event PropertyChangedEventHandler PropertyChanged;

        public void OnPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        #endregion
    }
}
