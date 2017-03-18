using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace UpdateDatabaseScript.Extension
{
    public class CommandAction : TriggerAction<DependencyObject>
    {


        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public object CommandParameter
        {
            get { return (object)GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }

        protected override void Invoke(object parameter)
        {
            if (Command != default(ICommand) && Command.CanExecute(CommandParameter))
                Command.Execute(CommandParameter);
        }

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register(
                "Command",
                typeof(ICommand),
                typeof(CommandAction),
                new PropertyMetadata(
                    default(ICommand)));

        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register(
                "CommandParameter",
                typeof(object),
                typeof(CommandAction),
                new UIPropertyMetadata(
                    default(object)));

    }
}
