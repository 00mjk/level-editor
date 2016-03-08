using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelEditor.Core.Settings.V1
{
    public class Location
    {
        public string ExportPath { get; set; }
        public int PathMode { get; set; }
        public string[] ExportPathPresets { get; set; }

        public static Location GenerateDefault()
        {
            return new Location
            {
                ExportPath = "default\\export\\path",
                PathMode = 1,
                ExportPathPresets = new[] { "C:\\absolute\\export\\path", "..\\relative\\export\\path" },
            };
        }
    }

    public class Resolution
    {
        public bool IsRelative { get; set; }
        public bool IsWidthBased { get; set; }
        public double Value { get; set; }

        public Resolution()
        {
            IsRelative = true;
            IsWidthBased = true;
            Value = 100.0;
        }

        public static Resolution GenerateDefault()
        {
            return new Resolution();
        }
    }

    public class Strip
    {
        public int Columns { get; set; }
        public int Rows { get; set; }
        public bool IsColumnMajor { get; set; }
        public int Margins { get; set; }
        public string BackgroundColor { get; set; }

        public Strip()
        {
            Columns = 1;
            IsColumnMajor = true;
            Margins = 10;
            BackgroundColor = "#FFFFFFFF";
        }

        public static Strip GenerateDefault()
        {
            return new Strip();
        }
    }

    public class Information
    {
        public int HorizontalAlignment { get; set; }
        public int VerticalAlignment { get; set; }
        public float Margin { get; set; }
        public int ShowParentPath { get; set; }
        public float FontSize { get; set; }
        public string TextColor { get; set; }
        public string TextOutlineColor { get; set; }
        public float TextOutlineThickness { get; set; }

        public static Information GenerateDefault()
        {
            return new Information
            {
                HorizontalAlignment = -1, // left
                VerticalAlignment = 1, // top
                Margin = 6, // 6 pixels from border
                ShowParentPath = 1, // show one level of parent directory
                FontSize = 14.0f,
                TextColor = "White",
                TextOutlineColor = "Black",
                TextOutlineThickness = 2.0f,
            };
        }
    }

    public class ScreenshotsSettings
    {
        public Location Location { get; set; }
        public Resolution Resolution { get; set; }
        public Strip Strip { get; set; }
        public Information Information { get; set; }

        public static ScreenshotsSettings GenerateDefault()
        {
            return new ScreenshotsSettings
            {
                Location = Location.GenerateDefault(),
                Resolution = Resolution.GenerateDefault(),
                Strip = Strip.GenerateDefault(),
                Information = Information.GenerateDefault(),
            };
        }
    }
}
