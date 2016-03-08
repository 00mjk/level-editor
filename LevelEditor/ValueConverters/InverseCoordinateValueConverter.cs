using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace LevelEditor.ValueConverters
{
    public class InverseCoordinateValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((value is double) == false)
                return value;

            return 1.0 - (double)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string)
            {
                double result;
                if (double.TryParse((string)value, out result))
                    return 1.0 - result;
            }
            else if (value is double)
                return 1.0 - (double)value;
            else if (value is float)
                return 1.0 - (float)value;

            return value;
        }
    }
}
