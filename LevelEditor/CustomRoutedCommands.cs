using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LevelEditor
{
    public static class CustomRoutedCommands
    {
        public static readonly ICommand DeselectAll = new RoutedCommand("DeselectAll", typeof(CustomRoutedCommands), new InputGestureCollection(new[] { new KeyGesture(Key.D, ModifierKeys.Control) }));
        public static readonly ICommand InvertSelection = new RoutedCommand("InvertSelection", typeof(CustomRoutedCommands), new InputGestureCollection(new[] { new KeyGesture(Key.I, ModifierKeys.Control) }));

        public static readonly ICommand EnableAllBlocks = new RoutedCommand("EnableAllBlocks", typeof(CustomRoutedCommands), new InputGestureCollection(new[] { new KeyGesture(Key.A, ModifierKeys.Control | ModifierKeys.Shift) }));
        public static readonly ICommand DisableAllBlocks = new RoutedCommand("DisableAllBlocks", typeof(CustomRoutedCommands), new InputGestureCollection(new[] { new KeyGesture(Key.D, ModifierKeys.Control | ModifierKeys.Shift) }));

        public static readonly ICommand GoToPreviousBlock = new RoutedCommand("GoToPreviousBlock", typeof(CustomRoutedCommands), new InputGestureCollection(new[] { new KeyGesture(Key.OemComma, ModifierKeys.Control) }));
        public static readonly ICommand GoToNextBlock = new RoutedCommand("GoToNextBlock", typeof(CustomRoutedCommands), new InputGestureCollection(new[] { new KeyGesture(Key.OemPeriod, ModifierKeys.Control) }));

        public static readonly ICommand GenerateGuid = new RoutedCommand("GenerateGuid", typeof(CustomRoutedCommands), new InputGestureCollection(new[] { new KeyGesture(Key.G, ModifierKeys.Control) }));
    }
}
