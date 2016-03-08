using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Bitcraft.UI.Core;
using LevelEditor.Extensibility;

namespace LevelEditor.ViewModels
{
    public class CanvasRendererViewModel : RootedViewModel, IDisposable
    {
        private CanvasRendererWrapper canvasRendererWrapper;
        public CanvasRendererWrapper Wrapper { get { return canvasRendererWrapper; } }

        public string Name { get; private set; }

        public event EventHandler EnabledChanged;

        protected virtual void OnEnabledChanged(EventArgs e)
        {
            var handler = EnabledChanged;
            if (handler != null)
                handler(this, e);
        }

        public bool IsEnabled
        {
            get { return canvasRendererWrapper.IsEnabled; }
            set
            {
                if (canvasRendererWrapper.IsEnabled != value)
                {
                    canvasRendererWrapper.IsEnabled = value;
                    if (Root.LayerData != null)
                        Root.LayerData.RequestRender();
                    OnEnabledChanged(EventArgs.Empty);
                }
            }
        }

        public CanvasRendererViewModel(RootViewModel root, CanvasRendererWrapper canvasRendererWrapper)
            : base(root)
        {
            if (canvasRendererWrapper == null)
                throw new ArgumentNullException("canvasRendererWrapper");

            this.canvasRendererWrapper = canvasRendererWrapper;

            Name = canvasRendererWrapper.CanvasRenderer.DisplayName;

            if (canvasRendererWrapper.CanvasRenderer.HasConfiguration)
                ConfigureCommand = new AnonymousCommand(canvasRendererWrapper.CanvasRenderer.Configure);
        }

        public ICommand ConfigureCommand { get; private set; }

        public void Dispose()
        {
            canvasRendererWrapper.Dispose();
        }
    }
}
