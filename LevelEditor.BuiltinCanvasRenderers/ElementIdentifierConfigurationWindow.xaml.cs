using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Bitcraft.UI.Core;

namespace LevelEditor.BuiltinCanvasRenderers
{
    /// <summary>
    /// Interaction logic for ElementIdentifierConfigurationWindow.xaml
    /// </summary>
    public partial class ElementIdentifierConfigurationWindow : Window
    {
        private ElementIdentifierCanvasRenderer renderer;

        private bool prevIsEnabled;
        private bool prevSelectedOnly;
        private string prevTypes;
        
        public ElementIdentifierConfigurationWindow(ElementIdentifierCanvasRenderer renderer)
        {
            InitializeComponent();
            Owner = Application.Current.MainWindow;

            DataContext = renderer;

            AcceptCommand = new AnonymousCommand(OnAccept);
            CancelCommand = new AnonymousCommand(OnCancel);

            prevIsEnabled = renderer.IsEnabled;
            prevSelectedOnly = renderer.SelectedOnly;
            prevTypes = renderer.Types;

            this.renderer = renderer;
        }

        public ICommand AcceptCommand { get; private set; }
        public ICommand CancelCommand { get; private set; }

        private bool cancelled = true;

        private void OnAccept()
        {
            // write to config file
            cancelled = false;
            Close();
        }

        private void OnCancel()
        {
            Close();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            if (cancelled)
            {
                renderer.IsEnabled = prevIsEnabled;
                renderer.SelectedOnly = prevSelectedOnly;
                renderer.Types = prevTypes;
            }
        }
    }
}
