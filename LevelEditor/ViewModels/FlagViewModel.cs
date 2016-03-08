using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LevelEditor.Extensibility.ReadOnlyViewModels;

namespace LevelEditor.ViewModels
{
    public class FlagViewModel : RootedViewModel, IReadOnlyFlagViewModel
    {
        public int FlagNumber { get; private set; }
        public string Semantic { get; private set; }

        private bool isChecked;
        public bool IsChecked
        {
            get { return isChecked; }
            set { SetDataValue(ref isChecked, value); }
        }

        public FlagViewModel(RootViewModel root, int flagNumber, string semantic, bool defaultIsCheckedValue)
            : base(root)
        {
            if (flagNumber < 0 || flagNumber > 256)
                throw new ArgumentException("Invalid 'flagNumber' argument. Must be between 0 and 255 included.", "flagNumber");
            if (string.IsNullOrWhiteSpace(semantic))
                throw new ArgumentException("Invalid 'semantic' argument. The string must not be null or contain only white spaces.", "semantic");

            FlagNumber = flagNumber;
            Semantic = semantic;
            IsChecked = defaultIsCheckedValue;
        }

        public FlagViewModel(RootViewModel root, int flagNumber, string semantic)
            : this(root, flagNumber, semantic, false)
        {
        }
    }
}
