using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace LevelEditor.Controls
{
    public class SmoothComboBox : ComboBox
    {
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            var popup = GetTemplateChild("PART_Popup") as Popup;

            if (popup == null)
                return;

            ScrollViewer scrollViewer = GetVisualChild<ScrollViewer>(popup.Child);

            if (scrollViewer != null)
                scrollViewer.CanContentScroll = false;
        }

        public static T GetVisualChild<T>(DependencyObject referenceVisual) where T : DependencyObject
        {
            DependencyObject child = null;

            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(referenceVisual); i++)
            {
                child = VisualTreeHelper.GetChild(referenceVisual, i) as DependencyObject;

                if (child != null && child is T)
                    break;

                child = GetVisualChild<T>(child);

                if (child != null && child is T)
                    break;
            }

            return child as T;
        }
    }
}
