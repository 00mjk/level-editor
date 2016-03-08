using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Bitcraft.UI.Core.Extensions;

namespace LevelEditor.Behaviors
{
    public class DoubleClickBehavior
    {
        public static object GetCommandParameter(DependencyObject target)
        {
            return target.GetValue(CommandParameterProperty);
        }

        public static void SetCommandParameter(DependencyObject target, object value)
        {
            target.SetValue(CommandParameterProperty, value);
        }

        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.RegisterAttached(
            "CommandParameter",
            typeof(object),
            typeof(DoubleClickBehavior),
            new PropertyMetadata(null));

        public static ICommand GetCommand(DependencyObject target)
        {
            return (ICommand)target.GetValue(CommandProperty);
        }

        public static void SetCommand(DependencyObject target, ICommand value)
        {
            target.SetValue(CommandProperty, value);
        }

        public static readonly DependencyProperty CommandProperty = DependencyProperty.RegisterAttached(
            "Command",
            typeof(ICommand),
            typeof(DoubleClickBehavior),
            new PropertyMetadata(OnDoubleClickCommandChanged));

        private static void OnDoubleClickCommandChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var control = sender as Control;

            if (control != null)
            {
                if (e.NewValue == null)
                {
                    control.MouseDoubleClick -= element_MouseDoubleClick;
                }
                else
                {
                    control.MouseDoubleClick += element_MouseDoubleClick;
                }
            }
            else
            {
                var element = (FrameworkElement)sender;
                if (element == null)
                    return;

                if (e.NewValue == null)
                {
                    element.MouseLeftButtonDown -= element_MouseLeftButtonDown;
                }
                else
                {
                    element.MouseLeftButtonDown += element_MouseLeftButtonDown;
                }
            }
        }

        private static void element_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                element_MouseDoubleClick(sender, e);
            }
        }

        private static void element_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var element = (FrameworkElement)sender;

            var command = GetCommand(element);
            if (command == null)
                return;

            command.ExecuteIfPossible(GetCommandParameter(element));
        }
    }
}
