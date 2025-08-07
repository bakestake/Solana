using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Huddle01.Helper 
{
    public class CustomValueNotifier<T> : INotifyPropertyChanged
    {
        private T _value;

        public event PropertyChangedEventHandler? PropertyChanged;

        public CustomValueNotifier(T initialValue)
        {
            _value = initialValue;
        }

        public T Value
        {
            get => _value;
            set
            {
                if (!EqualityComparer<T>.Default.Equals(_value, value))
                {
                    _value = value;
                    NotifyListeners();
                }
            }
        }

        protected void NotifyListeners()
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Value)));
        }

    }

    public class CustomValueNotifierHolder 
    {
        public static CustomValueNotifier<Dictionary<string, object>> PeersNotifier =
        new CustomValueNotifier<Dictionary<string, object>>(new Dictionary<string, object>());
    }

    

}


