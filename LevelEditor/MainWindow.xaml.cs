using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using Bitcraft.Core;
using Bitcraft.UI.Core.Extensions;
using LevelEditor.Core;
using LevelEditor.Core.Settings;
using LevelEditor.ViewModels;

namespace LevelEditor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public RootViewModel Root { get; private set; }

        public MainWindow()
        {
            InitializeComponent();

            Root = new RootViewModel();

            ApplicationCommands.SaveAs.InputGestures.Add(new KeyGesture(Key.S, ModifierKeys.Control | ModifierKeys.Shift));
            ApplicationCommands.Close.InputGestures.Add(new KeyGesture(Key.W, ModifierKeys.Control));

            CommandBindings.Add(CreateCommandBinding(ApplicationCommands.New, Root.FileNewMenuCommand));
            CommandBindings.Add(CreateCommandBinding(ApplicationCommands.Open, Root.FileOpenMenuCommand));
            CommandBindings.Add(CreateCommandBinding(ApplicationCommands.Save, Root.FileSaveMenuCommand));
            CommandBindings.Add(CreateCommandBinding(ApplicationCommands.SaveAs, Root.FileSaveAsMenuCommand));
            CommandBindings.Add(CreateCommandBinding(ApplicationCommands.Close, Root.FileCloseMenuCommand));

            CommandBindings.Add(CreateCommandBinding(ApplicationCommands.Delete, Root.SuppressCommand));

            CommandBindings.Add(CreateCommandBinding(ApplicationCommands.Cut, Root.CutCommand));
            CommandBindings.Add(CreateCommandBinding(ApplicationCommands.Copy, Root.CopyCommand));
            CommandBindings.Add(CreateCommandBinding(ApplicationCommands.Paste, Root.PasteCommand));

            CommandBindings.Add(CreateCommandBinding(ApplicationCommands.SelectAll, Root.SelectAllCommand));
            CommandBindings.Add(CreateCommandBinding(CustomRoutedCommands.DeselectAll, Root.DeselectAllCommand));
            CommandBindings.Add(CreateCommandBinding(CustomRoutedCommands.InvertSelection, Root.InvertSelectionCommand));

            CommandBindings.Add(CreateCommandBinding(CustomRoutedCommands.EnableAllBlocks, Root.EnableAllBlocksCommand));
            CommandBindings.Add(CreateCommandBinding(CustomRoutedCommands.DisableAllBlocks, Root.DisableAllBlocksCommand));

            CommandBindings.Add(CreateCommandBinding(CustomRoutedCommands.GoToPreviousBlock, Root.GoToPreviousBlockCommand));
            CommandBindings.Add(CreateCommandBinding(CustomRoutedCommands.GoToNextBlock, Root.GoToNextBlockCommand));

            CommandBindings.Add(CreateCommandBinding(CustomRoutedCommands.GenerateGuid, Root.GenerateGuidCommand));

            DataContext = Root;
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            base.OnClosing(e);

            Root.OnApplicationClose(new AnonymousCancellable(v => e.Cancel = v));
        }

        private CommandBinding CreateCommandBinding(ICommand source, ICommand target)
        {
            return new CommandBinding(source, (s, e) => target.ExecuteIfPossible(e.Parameter));
        }
    }
}
