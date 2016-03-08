using LevelEditor.Core.Settings.V1;
using LevelEditor.ViewModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace LevelEditor.ValueConverters
{
    public class LayerValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            if (value is byte)
            {
                var settings = ((MainWindow)Application.Current.MainWindow).Root.Settings;

                if (settings == null || settings.GameBoard == null || settings.GameBoard.LayerSettings == null)
                    return null;

                var layers = settings.GameBoard.LayerSettings.Layers;
                if (layers == null)
                    return null;

                var number = (byte)value;
                return layers.FirstOrDefault(x => x.Number == number);
            }

            throw new NotImplementedException();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;

            return ((Layer)value).Number;
        }
    }
}
