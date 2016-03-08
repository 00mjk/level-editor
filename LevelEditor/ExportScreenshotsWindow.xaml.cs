using LevelEditor.ViewModels;
using LevelEditor.ViewModels.ExportScreenshots;
using System;
using System.Collections.Generic;
using System.Linq;
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

namespace LevelEditor
{
    /// <summary>
    /// Interaction logic for ExportScreenshotsWindow.xaml
    /// </summary>
    public partial class ExportScreenshotsWindow : Window
    {
        public ExportScreenshotsWindow(ExportScreenshotsRootViewModel root)
        {
            InitializeComponent();
            this.DataContext = root;
        }
    }
}
