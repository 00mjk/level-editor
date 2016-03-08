using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LevelEditor.Extensibility;

namespace LevelEditor
{
    public class CanvasRendererWrapper : IDisposable
    {
        public ICanvasRenderer CanvasRenderer { get; private set; }

        public CanvasRendererWrapper(ICanvasRenderer canvasRenderer)
        {
            if (canvasRenderer == null)
                throw new ArgumentNullException("canvasRenderer");

            CanvasRenderer = canvasRenderer;
        }

        public bool IsEnabled { get; set; }

        public void Dispose()
        {
            var d = CanvasRenderer as IDisposable;
            if (d != null)
                d.Dispose();
        }
    }
}
