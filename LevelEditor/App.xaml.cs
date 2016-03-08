using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using LevelEditor.Extensibility;

namespace LevelEditor
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static readonly GlobalNotifyPropertyChanged GlobalPropertyChangedNotifier = new GlobalNotifyPropertyChanged();

        public const uint CurrentFileFormatVersion = 9;
    }

    public class GlobalNotifyPropertyChanged : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void Raise(object sender, string propertyName)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(sender, new PropertyChangedEventArgs(propertyName));
        }
    }
}
