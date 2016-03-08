using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace LevelEditor.Behaviors
{
    public class FocusOnLoadBehavior
    {
        public static bool GetIsAttached(FrameworkElement target)
        {
            return (bool)target.GetValue(IsAttachedProperty);
        }

        public static void SetIsAttached(FrameworkElement target, bool value)
        {
            target.SetValue(IsAttachedProperty, value);
        }

        public static readonly DependencyProperty IsAttachedProperty = DependencyProperty.RegisterAttached(
            "IsAttached",
            typeof(bool),
            typeof(FocusOnLoadBehavior),
            new PropertyMetadata(OnIsAttachedChanged));

        private static void OnIsAttachedChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var item = (FrameworkElement)sender;

            if (item.IsLoaded)
                MakeFocused(item);
            else
                item.Loaded += Item_Loaded;
        }

        private static void Item_Loaded(object sender, RoutedEventArgs e)
        {
            var item = (FrameworkElement)sender;
            item.Loaded -= Item_Loaded;
            MakeFocused(item);
        }

        private static void MakeFocused(FrameworkElement item)
        {
            item.Focus();
        }
    }
}
