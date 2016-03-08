using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LayerDataReaderWriter;

namespace LevelEditor.Core.Settings.V1
{
    public class LayerBlockElementDefinition
    {
        private const double BaseDefaultScaleX = 1.0;
        private const double BaseDefaultScaleY = 1.0;
        private const double BaseDefaultPivotX = 0.5;
        private const double BaseDefaultPivotY = 0.5;
        private const double BaseDefaultOutputDpiX = 96.0;
        private const double BaseDefaultOutputDpiY = 96.0;

        public string Categories { get; set; }
        public ushort Type { get; set; }
        public string IconFullPath { get; set; }
        public double DefaultScaleX { get; set; }
        public double DefaultScaleY { get; set; }
        public double DefaultPivotX { get; set; }
        public double DefaultPivotY { get; set; }
        public double DefaultOutputDpiX { get; set; }
        public double DefaultOutputDpiY { get; set; }

        public LayerBlockElementDefinition()
        {
            DefaultScaleX = BaseDefaultScaleX;
            DefaultScaleY = BaseDefaultScaleY;
            DefaultPivotX = BaseDefaultPivotX;
            DefaultPivotY = BaseDefaultPivotY;
            DefaultOutputDpiX = BaseDefaultOutputDpiX;
            DefaultOutputDpiY = BaseDefaultOutputDpiY;
        }

        public static LayerBlockElementDefinition GenerateDefault(string categories, ushort type, string iconFullPath)
        {
            return GenerateDefault(categories, type, iconFullPath, BaseDefaultScaleX, BaseDefaultScaleY);
        }

        public static LayerBlockElementDefinition GenerateDefault(string categories, ushort type, string iconFullPath, double defaultScaleX, double defaultScaleY)
        {
            return GenerateDefault(categories, type, iconFullPath, defaultScaleX, defaultScaleY, BaseDefaultPivotX, BaseDefaultPivotY);
        }

        public static LayerBlockElementDefinition GenerateDefault(string categories, ushort type, string iconFullPath, double defaultScaleX, double defaultScaleY, double defaultPivotX, double defaultPivotY)
        {
            return GenerateDefault(categories, type, iconFullPath, defaultScaleX, defaultScaleY, defaultPivotX, defaultPivotY, BaseDefaultOutputDpiX, BaseDefaultOutputDpiY);
        }

        public static LayerBlockElementDefinition GenerateDefault(string categories, ushort type, string iconFullPath, double defaultScaleX, double defaultScaleY, double defaultPivotX, double defaultPivotY, double defaultOutputDpiX, double defaultOutputDpiY)
        {
            return new LayerBlockElementDefinition
            {
                Categories = categories,
                Type = type,
                IconFullPath = iconFullPath,
                DefaultScaleX = defaultScaleX,
                DefaultScaleY = defaultScaleY,
                DefaultPivotX = defaultPivotX,
                DefaultPivotY = defaultPivotY,
                DefaultOutputDpiX = defaultOutputDpiX,
                DefaultOutputDpiY = defaultOutputDpiY
            };
        }
    }
}
