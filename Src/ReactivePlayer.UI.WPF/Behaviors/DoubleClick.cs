using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ReactivePlayer.UI.WPF.Behaviors
{
    // TODO: check if the behavior can be called LeftDoubleClick so we have Behaviors:LeftDoubleClick Command="{}" Modifier="{}"
    public class DoubleClick : DependencyObject
    {
        public static readonly DependencyProperty Command = DependencyProperty.RegisterAttached(
            nameof(Command),
            typeof(ICommand),
            typeof(DoubleClick),
            new PropertyMetadata(OnCommandChanged));
        public static ICommand GetCommand(Control target)
        {
            return (ICommand)target.GetValue(Command);
        }
        public static void SetCommand(Control target, ICommand value)
        {
            target.SetValue(Command, value);
        }

        public static readonly DependencyProperty CommandParameter = DependencyProperty.RegisterAttached(
            nameof(CommandParameter),
            typeof(object),
            typeof(DoubleClick));
        public static object GetCommandParameter(Control target)
        {
            return target.GetValue(CommandParameter);
        }
        public static void SetCommandParameter(Control target, object value)
        {
            target.SetValue(CommandParameter, value);
        }

        private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is Control control))
                return;

            ICommand command = GetCommand(control);

            if (command != null)
            {
                control.MouseDoubleClick += new MouseButtonEventHandler(MouseDoubleClickHandler);
            }
            else
            {
                control.MouseDoubleClick -= new MouseButtonEventHandler(MouseDoubleClickHandler);
            }
        }

        private static void MouseDoubleClickHandler(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is Control control))
                return;

            ICommand command = GetCommand(control);

            if (command == null)
                return;

            object commandParameter = GetCommandParameter(control);

            if (command.CanExecute(commandParameter))
            {
                command.Execute(commandParameter);
            }
        }
    }
}