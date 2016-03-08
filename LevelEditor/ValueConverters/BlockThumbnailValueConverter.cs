using LevelEditor.Controls;
using LevelEditor.Core;
using LevelEditor.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace LevelEditor.ValueConverters
{
    public class BlockThumbnailValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var layerBlock = (LayerBlockViewModel)value;

            var canvas = new Canvas();

            canvas.Background = new SolidColorBrush(Color.FromRgb(180, 180, 180));
            canvas.Width = layerBlock.Info.BlockWidth;
            canvas.Height = layerBlock.Info.BlockHeight;

            var geo = new GeometryHelper();

            foreach (var c in layerBlock.Instances)
            {
                var img = new Image();
                img.Source = ImageManager.GetImage(c.Type);
                img.RenderTransform = geo.GetElementTransform(layerBlock, c);
                canvas.Children.Add(img);
            }

            var size = new Size(layerBlock.Info.BlockWidth, layerBlock.Info.BlockHeight);
            canvas.Measure(size);
            canvas.Arrange(new Rect(size));

            var renderTarget = new RenderTargetBitmap((int)canvas.Width, (int)canvas.Height, 96.0, 96.0, PixelFormats.Pbgra32);
            renderTarget.Render(canvas);

            return renderTarget;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
