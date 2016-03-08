using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;
using LayerDataReaderWriter;
using LevelEditor.Core.Settings.V1;
using System.Windows.Media;

namespace LevelEditor.Core
{
    public static class ImageManager
    {
        private static string settingsFilePath;
        private static ProjectSettings settings;
        private static Dictionary<ushort, BitmapSource> images = new Dictionary<ushort, BitmapSource>();

        public static void Initialize(string projectSettingsFilePath, ProjectSettings projectSettings)
        {
            if (string.IsNullOrWhiteSpace(projectSettingsFilePath))
                throw new ArgumentException("Invalid 'projectSettingsFilePath' argument. The string must not be null or contain only white spaces.", "projectSettingsFilePath");

            if (projectSettings == null)
                throw new ArgumentNullException("projectSettings");

            settingsFilePath = projectSettingsFilePath;
            settings = projectSettings;

            images.Clear();
        }

        public static bool GetImageSize(ushort elementType, out Size size)
        {
            size = Size.Empty;

            BitmapSource image = GetImage(elementType);
            if (image == null)
                return false;

            size = new Size(image.Width, image.Height);
            return true;
        }

        public static bool GetImageDpi(ushort elementType, out double dpiX, out double dpiY, out double outputDpiX, out double outputDpiY)
        {
            dpiX = -1.0;
            dpiY = -1.0;
            outputDpiX = -1.0;
            outputDpiY = -1.0;

            BitmapSource image = GetImage(elementType);
            if (image == null)
                return false;

            LayerBlockElementDefinition foundDefinition = settings.GameElements.Elements.FirstOrDefault(def => def.Type == elementType);
            if (foundDefinition == null)
                return false;

            dpiX = image.DpiX;
            dpiY = image.DpiY;
            outputDpiX = foundDefinition.DefaultOutputDpiX;
            outputDpiY = foundDefinition.DefaultOutputDpiY;

            return true;
        }

        public static BitmapSource GetImage(ushort elementType)
        {
            if (settings == null)
                throw new InvalidOperationException("ImageManager must be initialized before used.");

            BitmapSource image;

            if (images.TryGetValue(elementType, out image) == false)
            {
                LayerBlockElementDefinition foundDefinition = settings.GameElements.Elements.FirstOrDefault(def => def.Type == elementType);
                if (foundDefinition != null)
                {
                    var iconFullPath = foundDefinition.IconFullPath;
                    if (Path.IsPathRooted(iconFullPath) == false)
                        iconFullPath = Path.GetFullPath(Path.Combine(settingsFilePath, iconFullPath));

                    image = LoadImage(iconFullPath);
                    if (image != null)
                        images.Add(foundDefinition.Type, image);
                }
            }

            return image;
        }

        /// <summary>
        /// Load the image contained in the given file.
        /// </summary>
        /// <param name="filename">Path and filename of the image to load, either absolute or relative to application executable.</param>
        /// <returns>Returns an image, or null if loading failed.</returns>
        private static BitmapSource LoadImage(string filename)
        {
            if (File.Exists(filename) == false)
            {
                System.Threading.ThreadPool.QueueUserWorkItem(_ =>
                    MessageBox.Show(string.Format("File '{0}' not found.", filename), "Exception", MessageBoxButton.OK, MessageBoxImage.Error), null);
                throw new FileNotFoundException(filename);
            }

            try
            {
                var bitmapImage = new BitmapImage();

                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
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

        public static BitmapSource CreateResizedBitmapFrame(ImageSource source, int width, int height)
        {
            var rect = new Rect(new Size(width, height));

            var drawing = new ImageDrawing(source, rect);
            RenderOptions.SetBitmapScalingMode(drawing, BitmapScalingMode.HighQuality);

            var drawingVisual = new DrawingVisual();
            using (var drawingContext = drawingVisual.RenderOpen())
                drawingContext.DrawDrawing(drawing);

            var resizedImage = new RenderTargetBitmap(width, height, 96.0, 96.0, PixelFormats.Default);
            resizedImage.Render(drawingVisual);

            return resizedImage;
        }
    }
}
