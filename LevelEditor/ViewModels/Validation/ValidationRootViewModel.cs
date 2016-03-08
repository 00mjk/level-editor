using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Bitcraft.UI.Core;
using LevelEditor.Extensibility;

namespace LevelEditor.ViewModels.Validation
{
    public class ValidationRootViewModel : RootedViewModel
    {
        public ValidatorViewModel[] Validators { get; private set; }

        public ValidationRootViewModel(RootViewModel root, IEnumerable<Lazy<IValidator, IValidatorMetadata>> validators)
            : base(root)
        {
            Validators = validators
                .Select(v => new ValidatorViewModel(root, v))
                .ToArray();

            RunAllCommand = new AnonymousCommand(OnRunAll);
        }

        public ICommand RunAllCommand { get; private set; }

        private void OnRunAll()
        {
            var data = Root.LayerData.Blocks
                .Select(b => b.ProduceLayerBlock())
                .ToArray();

            Parallel.ForEach(Validators, v => { v.ClearLog(); v.RunSynchronously(data); });
        }
    }
}
