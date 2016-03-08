using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelEditor.ViewModels.ExportScreenshots
{
    public class ExportInformationViewModel : RootedViewModel
    {
        public bool TopLeft { get; set; }
        public bool TopCenter { get; set; }
        public bool TopRight { get; set; }

        public bool CenterLeft { get; set; }
        public bool Center { get; set; }
        public bool CenterRight { get; set; }

        public bool BottomLeft { get; set; }
        public bool BottomCenter { get; set; }
        public bool BottomRight { get; set; }

        public float Margin { get; set; }
        public int ShowParentPath { get; set; }

        public float FontSize { get; set; }
        public string TextColor { get; set; }
        public string TextOutlineColor { get; set; }
        public float TextOutlineThickness { get; set; }

        public ExportInformationViewModel(RootViewModel root)
            : base(root)
        {
            // defaulty values
            TopLeft = true;
            Margin = 4.0f;
            ShowParentPath = 1;
            FontSize = 16.0f;
            TextColor = "Black";
            TextOutlineColor = "White";
            TextOutlineThickness = 1.0f;

            var s = root.Settings.Screenshots;
            if (s == null)
                return;

            var info = s.Information;
            if (info == null)
                return;

            TopLeft = false;

            if (info.VerticalAlignment == 0)
            {
                if (info.HorizontalAlignment == 0)
                    Center = true;
                else if (info.HorizontalAlignment < 0)
                    CenterLeft = true;
                else
                    CenterRight = true;
            }
            else if (info.VerticalAlignment > 0)
            {
                if (info.HorizontalAlignment == 0)
                    TopCenter = true;
                else if (info.HorizontalAlignment < 0)
                    TopLeft = true;
                else
                    TopRight = true;
            }
            else
            {
                if (info.HorizontalAlignment == 0)
                    BottomCenter = true;
                else if (info.HorizontalAlignment < 0)
                    BottomLeft = true;
                else
                    BottomRight = true;
            }

            Margin = info.Margin;
            ShowParentPath = info.ShowParentPath;

            FontSize = info.FontSize;
            TextColor = info.TextColor;
            TextOutlineColor = info.TextOutlineColor;
            TextOutlineThickness = info.TextOutlineThickness;
        }

        public void Initialize()
        {
        }
    }
}
