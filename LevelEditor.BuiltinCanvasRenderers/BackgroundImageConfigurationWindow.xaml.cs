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
    /// Interaction logic for BackgroundImageConfigurationWindow.xaml
    /// </summary>
    public partial class BackgroundImageConfigurationWindow : Window
    {
        private BackgroundImageCanvasRenderer renderer;

        private double prevOpacity;
        private string prevFilename;

        public BackgroundImageConfigurationWindow(BackgroundImageCanvasRenderer renderer)
        {
            InitializeComponent();
            Owner = Application.Current.MainWindow;

            DataContext = renderer;

            AcceptCommand = new AnonymousCommand(OnAccept);
            CancelCommand = new AnonymousCommand(OnCancel);

            prevOpacity = renderer.Opacity;
            prevFilename = renderer.Filename;

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
                renderer.Opacity = prevOpacity;
                renderer.Filename = prevFilename;
            }
        }
    }
}
