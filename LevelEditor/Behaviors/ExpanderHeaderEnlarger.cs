using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace LevelEditor.Behaviors
{
    public class ExpanderHeaderEnlarger
    {
        public static bool GetIsAttached(Expander target)
        {
            return (bool)target.GetValue(IsAttachedProperty);
        }

        public static void SetIsAttached(Expander target, bool value)
        {
            target.SetValue(IsAttachedProperty, value);
        }

        public static readonly DependencyProperty IsAttachedProperty = DependencyProperty.RegisterAttached(
            "IsAttached",
            typeof(bool),
            typeof(ExpanderHeaderEnlarger),
            new PropertyMetadata(OnIsAttachedChanged));

        private static void OnIsAttachedChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var expander = (Expander)sender;

            if (expander.IsLoaded)
                Process(expander);
            else
                expander.Loaded += Expander_Loaded;
        }

        private static void Expander_Loaded(object sender, RoutedEventArgs e)
        {
            var expander = (Expander)sender;
            expander.Loaded -= Expander_Loaded;
            Process(expander);
        }

        private static void Process(Expander expander)
        {
            var presenter = FindContentPresenter(expander);
            if (presenter != null)
                presenter.HorizontalAlignment = HorizontalAlignment.Stretch;
        }

        private static ContentPresenter FindContentPresenter(DependencyObject root)
        {
            if (root is ContentPresenter)
                return (ContentPresenter)root;

            foreach (var child in GetVisualChildren(root))
            {
                var test = FindContentPresenter(child);
                if (test != null)
                    return test;
            }

            return null;
        }

        private static IEnumerable<DependencyObject> GetVisualChildren(DependencyObject root)
        {
            var childCount = VisualTreeHelper.GetChildrenCount(root);
            for (int i = 0; i < childCount; i++)
                yield return VisualTreeHelper.GetChild(root, i);
        }
    }
}
