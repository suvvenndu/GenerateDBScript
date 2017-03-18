using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace _3Sharp.WpfLib
{
   public class RelayCommand: ICommand
    {
        Predicate<object> _canexecute = default(Predicate<object>);
        Action<object> _execute = default(Action<object>);


        public RelayCommand(Action<object> action) : this(action, null)
        {
            this._execute = action;
        }

        public RelayCommand(Action<object> action, Predicate<object> predicate)
        {
            this._execute = action;
            this._canexecute = predicate;
        }

        public bool CanExecute(object parameter)
        {
            return _canexecute == null ? true : _canexecute(parameter);
        }

        public event EventHandler CanExecuteChanged;

        public void Execute(object parameter)
        {
            _execute(parameter);
        }

    }
}
