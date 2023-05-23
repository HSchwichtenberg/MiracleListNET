using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using System.Windows.Input;

namespace MiracleList_WinUI.Behaviours
{
    public static class EnterKeyCommandBehaviour
    {
        public static ICommand GetCommand(TextBox obj)
        {
            return (ICommand)obj.GetValue(CommandProperty);
        }

        public static void SetCommand(TextBox obj, ICommand value)
        {
            obj.SetValue(CommandProperty, value);
        }

        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.RegisterAttached("Command",
                typeof(ICommand), typeof(EnterKeyCommandBehaviour),
                new PropertyMetadata(null, OnCommandChanged));

        private static void OnCommandChanged(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            var textBox = (TextBox)d;
            textBox.KeyDown -= TextBox_KeyDown;

            var command = e.NewValue as ICommand;
            if (command is not null)
            {
                textBox.KeyDown += TextBox_KeyDown;
            }
        }

        private static void TextBox_KeyDown(object sender,
            KeyRoutedEventArgs e)
        {
            var textBox = (TextBox)sender;
            var command = (ICommand)textBox.GetValue(CommandProperty);
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                var b = textBox.GetBindingExpression(TextBox.TextProperty);
                if (b != null)
                {
                    b.UpdateSource();
                }

                if (command.CanExecute(null))
                {
                    command.Execute(null);
                }
            }
        }
    }
}
