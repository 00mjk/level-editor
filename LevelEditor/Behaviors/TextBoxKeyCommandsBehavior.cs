using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using Bitcraft.UI.Core.Extensions;

namespace LevelEditor.Behaviors
{
    public enum LostFocusAction
    {
        Validate,
        Cancel,
        Nothing,
    }

    public class TextBoxKeyCommandsBehavior
    {
        // --- IsAttached -------------------------------------------

        public static bool GetIsAttached(DependencyObject target)
        {
            return (bool)target.GetValue(IsAttachedProperty);
        }

        public static void SetIsAttached(DependencyObject target, bool value)
        {
            target.SetValue(IsAttachedProperty, value);
        }

        public static readonly DependencyProperty IsAttachedProperty = DependencyProperty.RegisterAttached(
            "IsAttached",
            typeof(bool),
            typeof(TextBoxKeyCommandsBehavior),
            new PropertyMetadata(OnIsAttachedChanged));

        private static void OnIsAttachedChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var element = (FrameworkElement)sender;

            if ((bool)e.NewValue == false)
            {
                element.PreviewKeyDown -= element_PreviewKeyDown;
                element.LostFocus -= element_LostFocus;
            }
            else
            {
                element.PreviewKeyDown += element_PreviewKeyDown;
                element.LostFocus += element_LostFocus;
            }
        }

        private static void element_LostFocus(object sender, RoutedEventArgs e)
        {
            var obj = (DependencyObject)sender;

            var lostFocusAction = GetLostFocusAction(obj);
            var enterCommand = GetEnterCommand(obj);
            var escCommand = GetEscapeCommand(obj);

            if (lostFocusAction == LostFocusAction.Validate)
            {
                UpdateDataSource(obj);
                enterCommand.ExecuteIfPossible();
            }
            else if (lostFocusAction == LostFocusAction.Cancel)
                escCommand.ExecuteIfPossible();
        }

        private static void element_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            var obj = (DependencyObject)sender;

            if (e.Key == Key.Enter)
            {
                UpdateDataSource(obj);
                GetEnterCommand(obj).ExecuteIfPossible();
            }
            else if (e.Key == Key.Escape)
                GetEscapeCommand(obj).ExecuteIfPossible();
        }

        // --- LostFocusAction -------------------------------------------

        public static LostFocusAction GetLostFocusAction(DependencyObject target)
        {
            return (LostFocusAction)target.GetValue(LostFocusActionProperty);
        }

        public static void SetLostFocusAction(DependencyObject target, LostFocusAction value)
        {
            target.SetValue(LostFocusActionProperty, value);
        }

        public static readonly DependencyProperty LostFocusActionProperty = DependencyProperty.RegisterAttached(
            "LostFocusAction",
            typeof(LostFocusAction),
            typeof(TextBoxKeyCommandsBehavior),
            new PropertyMetadata(LostFocusAction.Validate));

        // --- EnterCommand -------------------------------------------

        public static ICommand GetEnterCommand(DependencyObject target)
        {
            return (ICommand)target.GetValue(EnterCommandProperty);
        }

        public static void SetEnterCommand(DependencyObject target, ICommand value)
        {
            target.SetValue(EnterCommandProperty, value);
        }

        public static readonly DependencyProperty EnterCommandProperty = DependencyProperty.RegisterAttached(
            "EnterCommand",
            typeof(ICommand),
            typeof(TextBoxKeyCommandsBehavior),
            new PropertyMetadata(null));

        // --- EscapeCommand -------------------------------------------

        public static ICommand GetEscapeCommand(DependencyObject target)
        {
            return (ICommand)target.GetValue(EscapeCommandProperty);
        }

        public static void SetEscapeCommand(DependencyObject target, ICommand value)
        {
            target.SetValue(EscapeCommandProperty, value);
        }

        public static readonly DependencyProperty EscapeCommandProperty = DependencyProperty.RegisterAttached(
            "EscapeCommand",
            typeof(ICommand),
            typeof(TextBoxKeyCommandsBehavior),
            new PropertyMetadata(null));

        // -------------------------------------------------------------

        private static void UpdateDataSource(DependencyObject source)
        {
            if (source == null)
                return;

            BindingExpression binding = BindingOperations.GetBindingExpression(source, TextBox.TextProperty);

            if (binding != null)
                binding.UpdateSource();
        }
    }
}
