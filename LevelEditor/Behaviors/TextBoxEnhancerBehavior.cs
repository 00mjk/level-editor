using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;

namespace LevelEditor.Behaviors
{
    public class TextBoxEnhancerBehavior
    {
        public static bool GetIsAttached(TextBox target)
        {
            return (bool)target.GetValue(IsAttachedProperty);
        }

        public static void SetIsAttached(TextBox target, bool value)
        {
            target.SetValue(IsAttachedProperty, value);
        }

        public static readonly DependencyProperty IsAttachedProperty = DependencyProperty.RegisterAttached(
            "IsAttached",
            typeof(bool),
            typeof(TextBoxEnhancerBehavior),
            new PropertyMetadata(OnIsAttachedChanged));

        private static void OnIsAttachedChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var textBox = (TextBox)sender;

            if ((bool)e.NewValue)
            {
                textBox.GotKeyboardFocus += textBox_GotKeyboardFocus;
                textBox.GotMouseCapture += textBox_GotMouseCapture;
                textBox.PreviewKeyDown += textBox_PreviewKeyDown;
            }
            else
            {
                textBox.GotKeyboardFocus -= textBox_GotKeyboardFocus;
                textBox.GotMouseCapture -= textBox_GotMouseCapture;
                textBox.PreviewKeyDown -= textBox_PreviewKeyDown;
            }
        }

        private static void textBox_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                UpdateDataSource(e.Source as DependencyObject);
        }

        private static void UpdateDataSource(DependencyObject source)
        {
            if (source == null)
                return;

            BindingExpression binding = BindingOperations.GetBindingExpression(source, TextBox.TextProperty);

            if (binding != null)
                binding.UpdateSource();
        }

        private static void textBox_GotMouseCapture(object sender, MouseEventArgs e)
        {
            var textBox = (TextBox)sender;
            textBox.SelectAll();
        }

        static void textBox_GotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs e)
        {
            var textBox = (TextBox)sender;
            textBox.SelectAll();
        }
    }
}
