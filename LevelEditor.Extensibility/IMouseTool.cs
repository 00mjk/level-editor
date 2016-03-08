using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using LevelEditor.Extensibility.ReadOnlyViewModels;

namespace LevelEditor.Extensibility
{
    public abstract class ToolBoxToolRenderer : Adorner
    {
        protected ToolBoxToolRenderer(UIElement adornedElement)
            : base(adornedElement)
        {
        }
    }

    public interface IMouseTool
    {
        ToolBoxToolRenderer GenerateRenderer(UIElement adornedElement);
        void SetBlock(IReadOnlyLayerBlockViewModel block);

        void MouseDown(Point position, MouseButtonEventArgs e, object item);
        void MouseMove(Point position, MouseEventArgs e);
        void MouseUp(Point position, MouseButtonEventArgs e);

        void KeyDown(KeyEventArgs e);
        void KeyUp(KeyEventArgs e);
    }
}
