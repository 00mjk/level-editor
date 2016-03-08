using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Bitcraft.UI.Core;
using LevelEditor.Controls;

namespace LevelEditor.Behaviors
{
    public class ShortcutKey : InputGesture
    {
        private Key key;

        public ShortcutKey(Key key)
        {
            this.key = key;
        }

        public override bool Matches(object targetElement, InputEventArgs inputEventArgs)
        {
            var e = inputEventArgs as KeyEventArgs;

            if (e == null)
                return false;

            if (e.Key != key || Keyboard.Modifiers != ModifierKeys.None)
                return false;

            return e.OriginalSource is GameBoardItemsControl;
        }
    }

    public class ToolBoxKeyBindingsBehavior
    {
        public static readonly DependencyProperty KeyCommandProperty = DependencyProperty.RegisterAttached(
            "KeyCommand",
            typeof(ICommand),
            typeof(ToolBoxKeyBindingsBehavior),
            new PropertyMetadata(OnKeyCommandChanged));

        private static bool runOnce = true;

        private static void OnKeyCommandChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (runOnce == false)
                return;

            var ctrl = Window.GetWindow(sender);
            if (ctrl == null)
                return;

            var command = GetKeyCommand((DependencyObject)sender);
            if (command == null)
                return;

            AddInputBinding(ctrl, command, Key.S);

            runOnce = true;
        }

        private static void AddInputBinding(Window window, ICommand command, Key key)
        {
            window.InputBindings.Add(new InputBinding(new AnonymousCommand(() => command.Execute(key)), new ShortcutKey(key)));
        }

        public static ICommand GetKeyCommand(DependencyObject target)
        {
            return (ICommand)target.GetValue(KeyCommandProperty);
        }

        public static void SetKeyCommand(DependencyObject target, ICommand value)
        {
            target.SetValue(KeyCommandProperty, value);
        }
    }
}
