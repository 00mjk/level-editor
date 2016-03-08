using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelEditor.ViewModels.ExportScreenshots
{
    public class ExportResolutionViewModel : RootedViewModel
    {
        private bool isRelative = true;
        public bool IsRelative
        {
            get { return isRelative; }
            set { SetValue(ref isRelative, value); }
        }

        private bool isWidthBased = true;
        public bool IsWidthBased
        {
            get { return isWidthBased; }
            set { SetValue(ref isWidthBased, value); }
        }

        private double value = 100.0;
        public double Value
        {
            get { return value; }
            set
            {
                var v = Math.Max(0, value);
                SetValue(ref this.value, v);
            }
        }

        public ExportResolutionViewModel(RootViewModel root)
            : base(root)
        {
            var s = root.Settings.Screenshots;

            if (s != null && s.Resolution != null)
            {
                IsRelative = s.Resolution.IsRelative;
                IsWidthBased = s.Resolution.IsWidthBased;
                Value = s.Resolution.Value;
            }
        }

        public void Initialize()
        {
        }
    }
}
