using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LevelEditor.Behaviors
{
    public class PreventScrollingBehavior
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
            typeof(PreventScrollingBehavior),
            new PropertyMetadata(OnIsAttachedChanged));

        private static void OnIsAttachedChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var item = (FrameworkElement)sender;

            if ((bool)e.NewValue)
                item.RequestBringIntoView += Item_RequestBringIntoView;
            else
                item.RequestBringIntoView -= Item_RequestBringIntoView;
        }

        private static void Item_RequestBringIntoView(object sender, RequestBringIntoViewEventArgs e)
        {
            e.Handled = true;
        }
    }
}
