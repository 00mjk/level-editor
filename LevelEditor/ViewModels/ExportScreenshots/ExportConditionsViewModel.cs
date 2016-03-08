using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelEditor.ViewModels.ExportScreenshots
{
    public class ExportConditionsViewModel : RootedViewModel
    {
        public bool ExportEnabledOnly { get; set; }

        public ExportConditionsViewModel(RootViewModel root)
            : base(root)
        {
            ExportEnabledOnly = false;
        }

        public void Initialize()
        {
        }
    }
}
