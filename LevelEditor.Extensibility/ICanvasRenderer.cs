using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using LevelEditor.Extensibility.ReadOnlyViewModels;

namespace LevelEditor.Extensibility
{
    public enum RenderPlace
    {
        Background,
        Foreground,
    }

    public interface ICanvasRenderer : IConfigurableExtension
    {
        string DisplayName { get; }
        RenderPlace RenderPlace { get; }
        void Initialize(IReadOnlyRootViewModel root, IGeometryHelper spriteHelper, INotifyPropertyChanged propertyChangedNotifier);
        void Render(DrawingContext drawingContext);
    }
}
