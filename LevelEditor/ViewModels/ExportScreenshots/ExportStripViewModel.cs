using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelEditor.ViewModels.ExportScreenshots
{
    public class ExportStripViewModel : RootedViewModel
    {
        private bool defineColumns = true;
        public bool DefineColumns
        {
            get { return defineColumns; }
            set { SetValue(ref defineColumns, value); }
        }

        private int columns;
        public int Columns
        {
            get { return columns; }
            set
            {
                var v = Math.Max(1, value);
                if (SetValue(ref columns, v))
                    ComputeBounds();
            }
        }

        private int rows;
        public int Rows
        {
            get { return rows; }
            set
            {
                var v = Math.Max(1, value);
                if (SetValue(ref rows, v) && skipCheck == false)
                    ComputeBounds();
            }
        }

        private bool isColumnMajor;
        public bool IsColumnMajor
        {
            get { return isColumnMajor; }
            set { SetValue(ref isColumnMajor, value); }
        }

        private int margins = 10;
        public int Margins
        {
            get { return margins; }
            set { SetValue(ref margins, value); }
        }

        private System.Windows.Media.Brush background = System.Windows.Media.Brushes.White;
        public System.Windows.Media.Brush Background
        {
            get { return background; }
            set { SetValue(ref background, value); }
        }

        private bool skipCheck;
        private int blockCount;

        public ExportStripViewModel(RootViewModel root)
            : base(root)
        {
            Initialize();

            var s = root.Settings.Screenshots;

            if (s != null && s.Strip != null)
            {
                if (s.Strip.Columns > 0)
                {
                    DefineColumns = true;
                    Columns = s.Strip.Columns;
                }
                else if (s.Strip.Rows > 0)
                {
                    DefineColumns = false;
                    Rows = s.Strip.Rows;
                }

                IsColumnMajor = s.Strip.IsColumnMajor;
                Margins = s.Strip.Margins;
                if (string.IsNullOrWhiteSpace(s.Strip.BackgroundColor) == false)
                {
                    try
                    {
                        var test = (System.Windows.Media.Brush)new System.Windows.Media.BrushConverter().ConvertFrom(s.Strip.BackgroundColor);
                        Background = test;
                    }
                    catch
                    {
                    }
                }
            }
        }

        public void Initialize()
        {
            if (Root.LayerData != null && Root.LayerData.Blocks != null)
                blockCount = Root.LayerData.Blocks.Count;

            ComputeBounds();
        }

        private void ComputeBounds()
        {
            if (skipCheck)
                return;

            skipCheck = true;
            if (DefineColumns)
                Rows = ComputeOtherBound(Columns);
            else
                Columns = ComputeOtherBound(Rows);
            skipCheck = false;
        }

        private int ComputeOtherBound(int value)
        {
            if (value <= 0)
                return 0;

            int rem;
            int other = Math.DivRem(blockCount, value, out rem);
            if (rem > 0)
                other++;
            return other;
        }
    }
}
