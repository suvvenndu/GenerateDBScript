using System;
using System.ComponentModel;
using System.Windows.Input;

namespace _3Sharp.WpfLib
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

       

        public void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != default(PropertyChangedEventHandler))
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
