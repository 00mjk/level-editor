using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LevelEditor.Behaviors
{
    public class UnfocusBehavior
    {
        public static bool GetIsAttached(FrameworkElement target)
        {
            return (bool)target.GetValue(IsAttachedProperty);
        }

        public static void SetIsAttached(FrameworkElement target, bool value)
        {
            target.SetValue(IsAttachedProperty, value);
        }

        public static readonly DependencyProperty IsAttachedProperty = DependencyProperty.RegisterAttached(
            "IsAttached",
            typeof(bool),
            typeof(UnfocusBehavior),
            new PropertyMetadata(OnIsAttachedChanged));

        private static void OnIsAttachedChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var element = (FrameworkElement)sender;
            element.PreviewMouseDown += Element_PreviewMouseDown;
        }

        private static void Element_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var window = Window.GetWindow((FrameworkElement)sender);
            FocusManager.SetFocusedElement(window, window);
        }
    }
}
