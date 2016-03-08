using LayerDataReaderWriter.V9;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Shapes;

namespace LevelEditor.ValueConverters
{
    public class ColliderImageValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((value is ColliderType) == false)
                return null;

            switch ((ColliderType)value)
            {
                case ColliderType.None: return new Rectangle { Width = 23.0, Height = 23.0, Stroke = Brushes.Black, StrokeThickness = 1.0, StrokeDashArray = new DoubleCollection(new[] { 4.0, 4.0 }), SnapsToDevicePixels = true };
                case ColliderType.Square: return new Rectangle { Width = 23.0, Height = 23.0, Fill = Brushes.Black };
                case ColliderType.Circle: return new Ellipse { Width = 23.0, Height = 23.0, Fill = Brushes.Black };
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
