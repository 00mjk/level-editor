using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
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
using LevelEditor.Extensibility;
using LevelEditor.ViewModels;
using LevelEditor.ViewModels.Validation;

namespace LevelEditor
{
    /// <summary>
    /// Interaction logic for ValidatorsWindow.xaml
    /// </summary>
    public partial class ValidatorsWindow : Window
    {
        private RootViewModel root;

        public ValidatorsWindow(RootViewModel root, IEnumerable<Lazy<IValidator, IValidatorMetadata>> validators)
        {
            if (root == null)
                throw new ArgumentNullException("root");
            if (validators == null)
                throw new ArgumentNullException("validators");

            this.root = root;

            InitializeComponent();

            this.DataContext = new ValidationRootViewModel(root, validators);
        }

        public void UpdateValidators(IEnumerable<Lazy<IValidator, IValidatorMetadata>> validators)
        {
            if (validators == null)
                throw new ArgumentNullException("validators");
            this.DataContext = new ValidationRootViewModel(root, validators);
        }
    }
}
