using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Bitcraft.UI.Core;
using LevelEditor.Extensibility;
using LevelEditor.Extensibility.ReadOnlyViewModels;

namespace LevelEditor.BuiltinCanvasRenderers
{
    [Export(typeof(ICanvasRenderer))]
    public class BackgroundImageCanvasRenderer : ViewModelBase, ICanvasRenderer
    {
        public string DisplayName { get { return "Background Image"; } }

        private double opacity = 1.0;
        public double Opacity
        {
            get { return opacity; }
            set
            {
                if (SetValue(ref opacity, value))
                    root.LayerData.RequestRender();
            }
        }

        private string filename;
        public string Filename
        {
            get { return filename; }
            set
            {
                if (SetValue(ref filename, value))
                {
                    image = LoadImage(filename);
                    root.LayerData.RequestRender();
                }
            }
        }

        private ImageSource image;

        private ImageSource LoadImage(string filename)
        {
            try
            {
                var bitmapImage = new BitmapImage();

                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.None;
                bitmapImage.UriSource = new Uri(filename, UriKind.Absolute);
                bitmapImage.EndInit();

                if (bitmapImage.CanFreeze)
                    bitmapImage.Freeze();

                return bitmapImage;
            }
            catch
            {
                return null;
            }
        }

        public RenderPlace RenderPlace
        {
            get { return RenderPlace.Background; }
        }

        private IReadOnlyRootViewModel root;

        public void Initialize(IReadOnlyRootViewModel root, IGeometryHelper spriteHelper, INotifyPropertyChanged propertyChangedNotifier)
        {
            if (root == null)
                throw new ArgumentNullException("root");

            this.root = root;
        }

        public void Render(DrawingContext drawingContext)
        {
            if (image == null)
                return;

            if (root.LayerData.SelectedBlock == null)
                return;

            var info = root.LayerData.SelectedBlock.Info;

            drawingContext.PushOpacity(Opacity);
            drawingContext.DrawImage(image, new Rect(0.0, 0.0, info.BlockWidth, info.BlockHeight));
            drawingContext.Pop();
        }

        public bool HasConfiguration { get { return true; } }

        public void Configure()
        {
            var window = new BackgroundImageConfigurationWindow(this);
            window.Show();
        }
    }
}
