using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using LevelEditor.Extensibility;

namespace LevelEditor.ValueConverters
{
    public class SeverityImageValueConverter : IValueConverter
    {
        public ImageSource Info { get; set; }
        public ImageSource Warning { get; set; }
        public ImageSource Error { get; set; }
        public ImageSource Fatal { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((value is SeverityLevel) == false)
                return null;

            switch ((SeverityLevel)value)
            {
                case SeverityLevel.Info: return Info;
                case SeverityLevel.Warning: return Warning;
                case SeverityLevel.Error: return Error;
                case SeverityLevel.Fatal: return Fatal;
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
