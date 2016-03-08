using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Bitcraft.Core;
using Bitcraft.Core.Interfaces;
using Bitcraft.UI.Core;
using Bitcraft.UI.Core.DataStateManagement;
using Bitcraft.UI.Core.Extensions;
using LayerDataReaderWriter;
using LayerDataReaderWriter.V9;
using LevelEditor.Core;
using LevelEditor.Core.Settings.V1;
using LevelEditor.Extensibility;
using LevelEditor.Extensibility.ReadOnlyViewModels;
using Microsoft.Win32;
using Bitcraft.Core.Utility;
using System.Diagnostics;
using System.Windows.Data;
using System.Windows.Controls;
using LevelEditor.ViewModels.ExportScreenshots;
using LevelEditor.Extensibility.ImportExport;
using LevelEditor.ViewModels.ImportExport;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Globalization;

namespace LevelEditor.ViewModels
{
    public class RootViewModel : ViewModelBase, IPartImportsSatisfiedNotification, IReadOnlyRootViewModel
    {
        private DataStateManager dataStateManager;

        private ProjectSettings settings;
        public ProjectSettings Settings
        {
            get { return settings; }
            private set { SetValue(ref settings, value); }
        }

        private LayerDataViewModel layerData;
        public LayerDataViewModel LayerData
        {
            get { return layerData; }
            private set { SetValue(ref layerData, value); }
        }

        private ComponentToolBoxViewModel componentToolBox;
        public ComponentToolBoxViewModel ComponentToolBox
        {
            get { return componentToolBox; }
            private set { SetValue(ref componentToolBox, value); }
        }

        private FeatureToolBoxViewModel featureToolBox;
        public FeatureToolBoxViewModel FeatureToolBox
        {
            get { return featureToolBox; }
            private set { SetValue(ref featureToolBox, value); }
        }

        private ElementToolBoxViewModel elementToolBox;
        public ElementToolBoxViewModel ElementToolBox
        {
            get { return elementToolBox; }
            private set { SetValue(ref elementToolBox, value); }
        }

        private bool hasLayerDataImporter;
        public bool HasLayerDataImporter
        {
            get { return hasLayerDataImporter; }
            private set { SetValue(ref hasLayerDataImporter, value); }
        }

        private bool hasLayerDataExporter;
        public bool HasLayerDataExporter
        {
            get { return hasLayerDataExporter; }
            private set { SetValue(ref hasLayerDataExporter, value); }
        }

        private bool hasLayerDataImporterOrExporter;
        public bool HasLayerDataImporterOrExporter
        {
            get { return hasLayerDataImporterOrExporter; }
            private set { SetValue(ref hasLayerDataImporterOrExporter, value); }
        }

        private ObservableCollection<CanvasRendererViewModel> canvasBackgroundRenderers = new ObservableCollection<CanvasRendererViewModel>();
        public IReadOnlyObservableCollection<CanvasRendererViewModel> CanvasBackgroundRenderers { get; private set; }

        private ObservableCollection<CanvasRendererViewModel> canvasForegroundRenderers = new ObservableCollection<CanvasRendererViewModel>();
        public IReadOnlyObservableCollection<CanvasRendererViewModel> CanvasForegroundRenderers { get; private set; }

        private ObservableCollection<LayerDataImporterViewModel> layerDataImporterViewModels = new ObservableCollection<LayerDataImporterViewModel>();
        public IReadOnlyObservableCollection<LayerDataImporterViewModel> LayerDataImporterViewModels { get; private set; }

        private ObservableCollection<LayerDataExporterViewModel> layerDataExporterViewModels = new ObservableCollection<LayerDataExporterViewModel>();
        public IReadOnlyObservableCollection<LayerDataExporterViewModel> LayerDataExporterViewModels { get; private set; }

        private BackupManager backupManager;

        [ImportMany(AllowRecomposition = true)]
        private IEnumerable<Lazy<IValidator, IValidatorMetadata>> validators = null;

        [ImportMany(AllowRecomposition = true)]
        private IEnumerable<ICanvasRenderer> canvasRenderers = null;

        [ImportMany(AllowRecomposition = true)]
        private IEnumerable<ILayerDataImporter> layerDataImporters = null;

        [ImportMany(AllowRecomposition = true)]
        private IEnumerable<ILayerDataExporter> layerDataExporters = null;

        private Version version;

        private string title;
        public string Title
        {
            get { return title; }
            set { SetValue(ref title, value); }
        }

        private ushort[] elementTypes;
        public ushort[] ElementTypes
        {
            get
            {
                if (elementTypes == null)
                    elementTypes = Settings.GameElements.Elements.Select(def => def.Type).ToArray();
                return elementTypes;
            }
        }

        public Layer[] Layers
        {
            get
            {
                if (Settings.GameBoard != null && Settings.GameBoard.LayerSettings != null && Settings.GameBoard.LayerSettings.Layers != null)
                    return Settings.GameBoard.LayerSettings.Layers;
                return new Layer[0];
            }
        }

        private ColliderType[] colliderTypes;
        public ColliderType[] ColliderTypes
        {
            get
            {
                if (colliderTypes == null)
                    colliderTypes = (ColliderType[])Enum.GetValues(typeof(ColliderType));
                return colliderTypes;
            }
        }

        private string extensionPath;
        private string canvasRenderersExtensionPath;
        private string importExportExtensionPath;

        private const string LastOpenedProjectFilename = ".last_opened_project.txt";
        private const string EnabledCanvasRenderersFilename = ".enabled_canvas_renderers.txt";

        public RootViewModel()
        {
            CanvasBackgroundRenderers = new Bitcraft.UI.Core.ReadOnlyObservableCollection<CanvasRendererViewModel>(canvasBackgroundRenderers);
            CanvasForegroundRenderers = new Bitcraft.UI.Core.ReadOnlyObservableCollection<CanvasRendererViewModel>(canvasForegroundRenderers);

            LayerDataImporterViewModels = new Bitcraft.UI.Core.ReadOnlyObservableCollection<LayerDataImporterViewModel>(layerDataImporterViewModels);
            LayerDataExporterViewModels = new Bitcraft.UI.Core.ReadOnlyObservableCollection<LayerDataExporterViewModel>(layerDataExporterViewModels);

            version = Assembly.GetEntryAssembly().GetName().Version;

            FileLoadProjectSettingsCommand = new AnonymousCommand(OnLoadProjectSettings);
            FileReloadProjectSettingsCommand = new AnonymousCommand(OnReloadProjectSettings);
            FileGenerateDefaultSettingsFile = new AnonymousCommand(OnGenerateDefaultSettingsFile);

            FileExportScreenCaptures = new AnonymousCommand(OnExportScreenCaptures);

            FileNewMenuCommand = new AnonymousCommand(OnFileNewMenu);
            FileOpenMenuCommand = new AnonymousCommand(OnFileOpenMenu);
            FileSaveMenuCommand = new AnonymousCommand(OnFileSaveMenu);
            FileSaveAsMenuCommand = new AnonymousCommand(OnFileSaveAsMenu);
            FileCloseMenuCommand = new AnonymousCommand(OnFileCloseMenu);

            ValidationCommand = new AnonymousCommand(OnValidationMenu);

            HelpAboutMenuCommand = new AnonymousCommand(OnHelpAboutMenu);

            SuppressCommand = new AnonymousCommand(OnSuppress);

            CutCommand = new AnonymousCommand(OnCut);
            CopyCommand = new AnonymousCommand(OnCopy);
            PasteCommand = new AnonymousCommand(OnPaste);
            SelectAllCommand = new AnonymousCommand(OnSelectAll);
            DeselectAllCommand = new AnonymousCommand(OnDeselectAll);
            InvertSelectionCommand = new AnonymousCommand(OnInvertSelection);

            EnableAllBlocksCommand = new AnonymousCommand(OnEnableAllBlocks);
            DisableAllBlocksCommand = new AnonymousCommand(OnDisableAllBlocks);

            GoToPreviousBlockCommand = new AnonymousCommand(OnGoToPreviousBlock);
            GoToNextBlockCommand = new AnonymousCommand(OnGoToNextBlock);

            GenerateGuidCommand = new AnonymousCommand(OnGenerateGuid);

            UpdateTitle(null, false);

            extensionPath = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Extensions"));
            if (Directory.Exists(extensionPath) == false)
                Directory.CreateDirectory(extensionPath);

            canvasRenderersExtensionPath = Path.Combine(extensionPath, "CanvasRenderers");
            if (Directory.Exists(canvasRenderersExtensionPath) == false)
                Directory.CreateDirectory(canvasRenderersExtensionPath);

            importExportExtensionPath = Path.Combine(extensionPath, "ImportExport");
            if (Directory.Exists(importExportExtensionPath) == false)
                Directory.CreateDirectory(importExportExtensionPath);

            RecomposeExtentionsCommand = new AnonymousCommand(RecomposeExtensions);

            var lastProjectInfoFile = Path.GetFullPath(LastOpenedProjectFilename);
            if (File.Exists(lastProjectInfoFile))
            {
                try
                {
                    var lastProjectPath = File.ReadAllText(lastProjectInfoFile);
                    if (string.IsNullOrWhiteSpace(lastProjectInfoFile) == false)
                        LoadSettings(lastProjectPath.Trim(), false);
                }
                catch
                {
                }
            }

            RecomposeExtensions();

            //LoadSettings(Path.GetFullPath(@"..\..\..\..\showcaseapp2\LevelEditorSettings\cube.settings"));

            /*
            var filename = Path.GetFullPath(@"..\..\..\showcaseapp2\Unity\Assets\Resources\Levels\Levels_W01_L001_V01.bytes");
            LayerData.LoadData(filename, (LayerFile)ReaderWriterManager.Read(File.OpenRead(filename), Encoding.UTF8, App.CurrentFileFormatVersion));
            IsDataModified = false;
            */
        }

        private void OnLoadProjectSettings()
        {
            var dlg = new OpenFileDialog
            {
                CheckFileExists = true,
                CheckPathExists = true,
                Filter = "Project Files (*.settings)|*.settings|All Files (*.*)|*.*",
                Multiselect = false,
                ShowReadOnly = true,
                Title = "Load project settings file",
            };

            if (dlg.ShowDialog() != true)
                return;

            LoadSettings(dlg.FileName, true);
        }

        private bool hasCurrentProject;
        public bool HasCurrentProject
        {
            get { return hasCurrentProject; }
            private set { SetValue(ref hasCurrentProject, value); }
        }

        private void OnReloadProjectSettings()
        {
            if (string.IsNullOrWhiteSpace(projectFilename))
                return;
            LoadSettings(projectFilename, true);
        }

        private void OnGenerateDefaultSettingsFile()
        {
            var dlg = new SaveFileDialog
            {
                CheckPathExists = true,
                Filter = "Project Files (*.settings)|*.settings|All Files (*.*)|*.*",
                Title = "Save project settings file",
            };

            if (dlg.ShowDialog() != true)
                return;

            ProjectSettings settings = null;

            try
            {
                settings = ProjectSettings.GenerateDefault();
            }
            catch
            {
                MessageBox.Show("Failed to generate project settings file.", "Creation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                settings.Save(dlg.FileName);
            }
            catch
            {
                MessageBox.Show("Failed to save project settings file.", "Save Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private ExportScreenshotsRootViewModel exportScreenshotsRootViewModel;

        private void OnExportScreenCaptures()
        {
            if (Settings == null)
            {
                MessageBox.Show("Load project settings first.", "Project settings required", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            if (exportScreenshotsRootViewModel == null)
                exportScreenshotsRootViewModel = new ExportScreenshotsRootViewModel(this);

            var exportWindow = new ExportScreenshotsWindow(exportScreenshotsRootViewModel);
            exportWindow.Owner = Application.Current.MainWindow;

            exportScreenshotsRootViewModel.SetWindowControls(
                    () => exportWindow.DialogResult = true,
                    () => exportWindow.DialogResult = false);

            exportScreenshotsRootViewModel.Initialize();

            var dlgResult = exportWindow.ShowDialog();

            if (dlgResult != true)
                return;

            var export = exportScreenshotsRootViewModel;

            var basePath = export.ExportLocation.ResolveToPath;

            if (Directory.Exists(basePath) == false)
                Directory.CreateDirectory(basePath);

            var baseName = Path.GetFileNameWithoutExtension(LayerData.FullFilename);

            LayerData.WorkInProgressRate = 0.0;
            LayerData.WorkInProgress = true;

            var columnCount = export.ExportStrip.Columns;
            var rowCount = export.ExportStrip.Rows;

            var strip = new BitmapSource[columnCount, rowCount];

            var column = 0;
            var row = 0;

            var n = 1;
            foreach (var block in LayerData.Blocks)
            {
                if (export.ExportConditions.ExportEnabledOnly && block.IsEnabled == false)
                    continue;

                LayerData.WorkInProgressText = string.Format("Rendering block {0} / {1}", n, LayerData.Blocks.Count);

                LayerData.SelectedBlock = block;
                LayerData.RequestRender();

                System.Windows.Threading.Dispatcher.CurrentDispatcher.Invoke(delegate
                {
                    var filename = Path.Combine(basePath, string.Format("{0}_{1:D3}.png", baseName, block.Identifier));
                    var image = LayerData.RequestScreenshot();

                    var ratio = image.PixelWidth / (double)image.PixelHeight;

                    double newWidth;
                    double newHeight;

                    if (export.ExportResolution.IsWidthBased)
                    {
                        if (export.ExportResolution.IsRelative)
                            newWidth = image.PixelWidth * export.ExportResolution.Value / 100.0;
                        else
                            newWidth = export.ExportResolution.Value;

                        newHeight = newWidth / ratio;
                    }
                    else
                    {
                        if (export.ExportResolution.IsRelative)
                            newHeight = image.PixelHeight * export.ExportResolution.Value / 100.0;
                        else
                            newHeight = export.ExportResolution.Value;

                        newWidth = newHeight * ratio;
                    }

                    image = ImageManager.CreateResizedBitmapFrame(image, (int)Math.Round(newWidth), (int)Math.Round(newHeight));

                    image = PrintInfo(filename, block.Identifier, export.ExportInformation, image);

                    strip[column, row] = image;

                    if (export.ExportStrip.IsColumnMajor)
                    {
                        column++;
                        if (column >= export.ExportStrip.Columns)
                        {
                            row++;
                            column = 0;
                        }
                    }
                    else
                    {
                        row++;
                        if (row >= export.ExportStrip.Rows)
                        {
                            column++;
                            row = 0;
                        }
                    }

                    var pngEncoder = new PngBitmapEncoder();
                    pngEncoder.Frames.Add(BitmapFrame.Create(image));
                    using (var stream = new FileStream(filename, FileMode.Create, FileAccess.Write))
                        pngEncoder.Save(stream);

                }, System.Windows.Threading.DispatcherPriority.Render);

                LayerData.WorkInProgressRate = n / (double)LayerData.Blocks.Count;

                n++;
            }

            LayerData.WorkInProgressRate = 1.0;
            LayerData.WorkInProgressText = "Generating the strip";

            System.Windows.Threading.Dispatcher.CurrentDispatcher.Invoke(delegate
            {
                var maxColumnWidth = 0;
                var maxRowHeight = 0;

                for (var c = 0; c < columnCount; c++)
                {
                    for (var r = 0; r < rowCount; r++)
                    {
                        var img = strip[c, r];
                        if (img == null)
                            continue;

                        maxColumnWidth = Math.Max(maxColumnWidth, img.PixelWidth);
                        maxRowHeight = Math.Max(maxRowHeight, img.PixelHeight);
                    }
                }

                var totalWidth = export.ExportStrip.Margins * (2 + columnCount - 1) + columnCount * maxColumnWidth;
                var totalHeight = export.ExportStrip.Margins * (2 + rowCount - 1) + rowCount * maxRowHeight;

                var dv = new DrawingVisual();
                var rect = new Rect(new Size(totalWidth, totalHeight));

                using (DrawingContext ctx = dv.RenderOpen())
                {
                    ctx.DrawRectangle(export.ExportStrip.Background, null, rect); // background of the parent control

                    var x = export.ExportStrip.Margins;
                    var y = export.ExportStrip.Margins;

                    for (var c = 0; c < columnCount; c++)
                    {
                        for (var r = 0; r < rowCount; r++)
                        {
                            var img = strip[c, r];
                            if (img == null)
                                continue;

                            var offsetX = (maxColumnWidth - img.PixelWidth) / 2;
                            var offsetY = (maxRowHeight - img.PixelHeight) / 2;

                            ctx.DrawRectangle(new ImageBrush(img), null, new Rect(x + offsetX, y + offsetY, img.PixelWidth, img.PixelHeight));
                            ctx.DrawRectangle(null, new Pen(Brushes.Gray, 1), new Rect(x + offsetX, y + offsetY, img.PixelWidth, img.PixelHeight));

                            y += maxRowHeight + export.ExportStrip.Margins;
                        }
                        y = export.ExportStrip.Margins;
                        x += maxColumnWidth + export.ExportStrip.Margins;
                    }
                }

                var stripImage = new System.Windows.Media.Imaging.RenderTargetBitmap(totalWidth, totalHeight, 96.0, 96.0, System.Windows.Media.PixelFormats.Pbgra32);
                stripImage.Render(dv);

                var stripFilename = Path.Combine(basePath, string.Format("{0}_strip.png", baseName));

                var pngEncoder = new System.Windows.Media.Imaging.PngBitmapEncoder();
                pngEncoder.Frames.Add(System.Windows.Media.Imaging.BitmapFrame.Create(stripImage));
                using (var stream = new FileStream(stripFilename, FileMode.Create, FileAccess.Write))
                    pngEncoder.Save(stream);

            }, System.Windows.Threading.DispatcherPriority.Render);


            LayerData.WorkInProgress = false;
        }

        private static readonly Typeface DefaultTypeface = new Typeface(new FontFamily("arial"), FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);

        private BitmapSource PrintInfo(string filename, int blockId, ExportInformationViewModel exportInfo, BitmapSource image)
        {
            var parts = filename.Split(new char[] { '/', '\\' }, StringSplitOptions.RemoveEmptyEntries);
            var partCount = Math.Max(1, Math.Min(exportInfo.ShowParentPath + 1, parts.Length));

            var filenameText = string.Format("{0} - Block {1}", string.Join("\\", parts, parts.Length - partCount, partCount), blockId);

            var imageWidth = image.PixelWidth;
            var imageHeight = image.PixelHeight;

            var brushConverter = new BrushConverter();

            Brush textBrsuh;
            Pen textOutlinePen;

            try
            {
                textBrsuh = (Brush)brushConverter.ConvertFromString(exportInfo.TextColor);
            }
            catch
            {
                textBrsuh = Brushes.Black;
            }

            try
            {
                textOutlinePen = new Pen((Brush)brushConverter.ConvertFromString(exportInfo.TextOutlineColor), exportInfo.TextOutlineThickness);
            }
            catch
            {
                textOutlinePen = new Pen(Brushes.White, exportInfo.TextOutlineThickness);
            }

            var drawingVisual = new DrawingVisual();
            using (DrawingContext drawingContext = drawingVisual.RenderOpen())
            {
                drawingContext.DrawImage(image, new Rect(new Size(imageWidth, imageHeight)));

                var text = new FormattedText(filenameText, CultureInfo.InvariantCulture, FlowDirection.LeftToRight, DefaultTypeface, exportInfo.FontSize > 0.0f ? exportInfo.FontSize : 14.0f, Brushes.Black);

                var textWidth = text.Width;
                var textHeight = text.Height;

                var pos = new Point();

                if (exportInfo.TopLeft)
                    pos = new Point(exportInfo.Margin, exportInfo.Margin);
                else if (exportInfo.TopCenter)
                    pos = new Point((imageWidth - textWidth) / 2.0, exportInfo.Margin);
                else if (exportInfo.TopRight)
                    pos = new Point(imageWidth - (textWidth + exportInfo.Margin), exportInfo.Margin);

                else if (exportInfo.CenterLeft)
                    pos = new Point(exportInfo.Margin, (imageHeight - textHeight) / 2.0);
                else if (exportInfo.Center)
                    pos = new Point((imageWidth - textWidth) / 2.0, (imageHeight - textHeight) / 2.0);
                else if (exportInfo.CenterRight)
                    pos = new Point(imageWidth - (textWidth + exportInfo.Margin), (imageHeight - textHeight) / 2.0);

                else if (exportInfo.BottomLeft)
                    pos = new Point(exportInfo.Margin, imageHeight - (textHeight + exportInfo.Margin));
                else if (exportInfo.BottomCenter)
                    pos = new Point((imageWidth - textWidth) / 2.0, imageHeight - (textHeight + exportInfo.Margin));
                else if (exportInfo.BottomRight)
                    pos = new Point(imageWidth - (textWidth + exportInfo.Margin), imageHeight - (textHeight + exportInfo.Margin));

                Geometry geometry = text.BuildGeometry(pos);

                drawingContext.DrawGeometry(null, textOutlinePen, geometry);
                drawingContext.DrawGeometry(textBrsuh, null, geometry);
            }

            var rt = new RenderTargetBitmap(imageWidth, imageHeight, image.DpiX, image.DpiY, image.Format);
            rt.Render(drawingVisual);
            rt.Freeze();

            return rt;
        }

        private string projectFilename;

        private void LoadSettings(string filename, bool isManual)
        {
            if (dataStateManager != null && dataStateManager.Close() == false)
                return;

            try
            {
                Settings = ProjectSettings.Load(filename);
                projectFilename = filename;
                HasCurrentProject = true;
            }
            catch (Exception ex)
            {
                if (isManual)
                    MessageBox.Show("Error loading settings", ex.ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var projectSettingsFilePath = Path.GetDirectoryName(filename);

            if (CheckImageFilesAvailability(projectSettingsFilePath, settings) == false)
                return;

            ImageManager.Initialize(projectSettingsFilePath, Settings);

            LayerData = new LayerDataViewModel(this);

            ComponentToolBox = new ComponentToolBoxViewModel(this);
            FeatureToolBox = new FeatureToolBoxViewModel(this);
            ElementToolBox = new ElementToolBoxViewModel(this);

            backupManager = new BackupManager(this);

            dataStateManager = new DataStateManager(
                new LevelEditorDataStateProcessor(this),
                new WindowsDialogDataStateViewProcessor("Binary Layer Data Files (*.bytes)|*.bytes|All Files (*.*)|*.*"));

            if (validatorsWindow != null)
                validatorsWindow.UpdateValidators(validators);

            // force re-evaluation of element types
            elementTypes = null;

            IsDataModified = true;
            IsDataModified = false; // ugly trick to fix an update issue

            if (isManual)
            {
                try
                {
                    File.WriteAllText(Path.GetFullPath(LastOpenedProjectFilename), filename);
                }
                catch
                {
                }
            }
        }

        private bool CheckImageFilesAvailability(string projectSettingsFilePath, ProjectSettings projectSettings)
        {
            var missings = projectSettings.GameElements.Elements
                .Select(x => new { Defined = x.IconFullPath, Resolved = Path.GetFullPath(Path.Combine(projectSettingsFilePath, x.IconFullPath)) })
                .Where(x => File.Exists(x.Resolved) == false)
                .ToArray();

            if (missings.Length == 0)
                return true;

            string result = TextTable.Create(missings)
                .Column("Defined", x => x.Defined, ColumnHorizontalAlignment.Left)
                .Column("Resolved to", x => x.Resolved, ColumnHorizontalAlignment.Left)
                .Generate();

            var filename = "MissingImageFilesReport.txt";
            var fullFilename = Path.GetFullPath(filename);

            var baseMessage = string.Format("Image file{0} missing", missings.Length == 1 ? " is" : "s are");
            string message = null;

            try
            {
                File.WriteAllText(fullFilename, result);
                message = string.Format("{0}, check the file '{1}' for the full listing.", baseMessage, filename);
            }
            catch
            {
                message = string.Format("{0}:{1}{2}", baseMessage, Environment.NewLine, result);
            }

            MessageBox.Show(message, "Resource Error", MessageBoxButton.OK, MessageBoxImage.Error);

            return false;
        }

        public void UpdateTitle(string filename)
        {
            UpdateTitle(filename, IsDataModified);
        }

        public void UpdateTitle(string filename, bool isModified)
        {
            var sb = new StringBuilder();

            var versionStr = string.Format("{0}.{1}", version.Major, version.Minor);
            if (version.Revision > 0)
                versionStr += string.Format(".{0}.{1}", version.Build, version.Revision);
            else if (version.Build > 0)
                versionStr += "." + version.Build;

            sb.AppendFormat("bitcraft Level Editor {0}", versionStr);

            var hasProject = string.IsNullOrWhiteSpace(projectFilename) == false;
            var hasFile = string.IsNullOrWhiteSpace(filename) == false;

            if (hasProject || hasFile)
                sb.AppendFormat(" - ");

            if (hasProject)
            {
                sb.AppendFormat("[{0}]", Path.GetFileNameWithoutExtension(projectFilename));
                if (hasFile)
                    sb.Append(" ");
            }

            if (hasFile)
            {
                if (filename.Length > 50)
                    filename = string.Format("...{0}", filename.Substring(filename.Length - 47));
                sb.AppendFormat("{0}", filename);
            }

            if (isModified)
                sb.Append(" *");

            Title = sb.ToString();
        }

        public AnonymousCommand FileLoadProjectSettingsCommand { get; private set; }
        public AnonymousCommand FileReloadProjectSettingsCommand { get; private set; }
        public AnonymousCommand FileGenerateDefaultSettingsFile { get; private set; }

        public AnonymousCommand FileExportScreenCaptures { get; private set; }

        public AnonymousCommand FileNewMenuCommand { get; private set; }
        public AnonymousCommand FileOpenMenuCommand { get; private set; }
        public AnonymousCommand FileSaveMenuCommand { get; private set; }
        public AnonymousCommand FileSaveAsMenuCommand { get; private set; }
        public AnonymousCommand FileCloseMenuCommand { get; private set; }

        public AnonymousCommand RecomposeExtentionsCommand { get; private set; }
        public AnonymousCommand ValidationCommand { get; private set; }

        public AnonymousCommand HelpAboutMenuCommand { get; private set; }

        public AnonymousCommand SuppressCommand { get; private set; }

        public AnonymousCommand CutCommand { get; private set; }
        public AnonymousCommand CopyCommand { get; private set; }
        public AnonymousCommand PasteCommand { get; private set; }

        public AnonymousCommand SelectAllCommand { get; private set; }
        public AnonymousCommand DeselectAllCommand { get; private set; }
        public AnonymousCommand InvertSelectionCommand { get; private set; }

        public AnonymousCommand EnableAllBlocksCommand { get; private set; }
        public AnonymousCommand DisableAllBlocksCommand { get; private set; }

        public AnonymousCommand GoToPreviousBlockCommand { get; private set; }
        public AnonymousCommand GoToNextBlockCommand { get; private set; }

        public AnonymousCommand GenerateGuidCommand { get; set; }

        public bool IsDataModified
        {
            get { return dataStateManager.IsModified; }
            set
            {
                if (backupManager != null)
                    backupManager.OnDataChanged();

                if (dataStateManager != null)
                    dataStateManager.IsModified = value;
            }
        }

        private SaveStatus saveStatus;

        public SaveStatus SaveStatus
        {
            get { return saveStatus; }
            set
            {
                if (SetValue(ref saveStatus, value) && saveStatus == SaveStatus.SaveSucceeded || saveStatus == SaveStatus.SaveFailed)
                    ((Action)(async () => { await Task.Delay(2000); SaveStatus = SaveStatus.None; }))();
            }
        }

        // THIS SHOULD NOT BE IN A VIEW MODEL
        private void ForceFocusedTextBoxToBind()
        {
            var focused = Keyboard.FocusedElement as TextBox;
            if (focused == null)
                return;

            BindingExpression bindingExpression = focused.GetBindingExpression(TextBox.TextProperty);
            if (bindingExpression == null)
                return;

            bindingExpression.UpdateSource();
        }

        public void ImportData(string filename, LayerFile data)
        {
            LayerData.LoadData(filename, data);
            IsDataModified = false;

            dataStateManager.Open(new FileStream(filename, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite));
        }

        private void FileOperation(Action action)
        {
            SaveStatus = SaveStatus.None;

            ForceFocusedTextBoxToBind();

            try
            {
                action();
                if (SaveStatus == SaveStatus.Saving)
                    SaveStatus = SaveStatus.SaveSucceeded;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString(), "File Error", MessageBoxButton.OK, MessageBoxImage.Error);
                if (SaveStatus == SaveStatus.Saving)
                    SaveStatus = SaveStatus.SaveFailed;
            }
        }

        private void OnFileNewMenu()
        {
            if (dataStateManager == null)
                return;

            FileOperation(() => dataStateManager.Close());
        }

        private void OnFileOpenMenu()
        {
            if (dataStateManager == null)
            {
                MessageBox.Show("Load project settings first.", "Project settings required", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            FileOperation(dataStateManager.Open);
        }

        private void OnFileSaveMenu()
        {
            if (dataStateManager == null)
            {
                MessageBox.Show("Load project settings first.", "Project settings required", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            FileOperation(dataStateManager.Save);
        }

        private void OnFileSaveAsMenu()
        {
            if (dataStateManager == null)
            {
                MessageBox.Show("Load project settings first.", "Project settings required", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            FileOperation(dataStateManager.SaveAs);
        }

        private void OnFileCloseMenu()
        {
            App.Current.MainWindow.Close();
        }

        public void OnApplicationClose(ICancellable state)
        {
            if (dataStateManager == null)
                return;

            FileOperation(() =>
            {
                if (dataStateManager.Close() == false)
                    state.IsCancelled = true;
            });
        }

        private ValidatorsWindow validatorsWindow;

        private void OnValidationMenu()
        {
            if (validators == null)
                return;

            validatorsWindow = new ValidatorsWindow(this, validators);
            validatorsWindow.Owner = Application.Current.MainWindow;
            validatorsWindow.Closed += (ss, ee) => validatorsWindow = null;
            validatorsWindow.Show();
        }

        private void OnHelpAboutMenu()
        {
            var sb = new StringBuilder();

            var versionInfo = FileVersionInfo.GetVersionInfo(Assembly.GetEntryAssembly().Location);

            sb.AppendLine(string.Format("LevelEditor v{0}", Assembly.GetEntryAssembly().GetName().Version));
            sb.AppendLine();
            sb.AppendLine(versionInfo.CompanyName);
            sb.AppendLine(versionInfo.LegalCopyright);
            sb.AppendLine();
            sb.AppendLine("contact@bitcraft.co.jp");

            MessageBox.Show(sb.ToString(), "About LevelEditor", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void OnSuppress()
        {
            if (LayerData.SelectedBlock != null)
                LayerData.SelectedBlock.SuppressCommand.ExecuteIfPossible();
        }

        private void OnCut()
        {
            if (LayerData == null)
                return;

            if (LayerData.SelectedBlock != null)
                LayerData.SelectedBlock.CutCommand.ExecuteIfPossible();
        }

        private void OnCopy()
        {
            if (LayerData == null)
                return;

            if (LayerData.SelectedBlock != null)
                LayerData.SelectedBlock.CopyCommand.ExecuteIfPossible();
        }

        private void OnPaste()
        {
            if (LayerData == null)
                return;

            if (LayerData.SelectedBlock != null)
                LayerData.SelectedBlock.PasteCommand.ExecuteIfPossible();
        }

        private void OnSelectAll()
        {
            if (LayerData == null)
                return;

            if (LayerData.SelectedBlock != null)
                LayerData.SelectedBlock.SelectAllCommand.ExecuteIfPossible();
        }

        private void OnDeselectAll()
        {
            if (LayerData == null)
                return;

            if (LayerData.SelectedBlock != null)
                LayerData.SelectedBlock.DeselectAllCommand.ExecuteIfPossible();
        }

        private void OnInvertSelection()
        {
            if (LayerData == null)
                return;

            if (LayerData.SelectedBlock != null)
                LayerData.SelectedBlock.InvertSelectionCommand.ExecuteIfPossible();
        }

        private void OnEnableAllBlocks()
        {
            if (LayerData == null)
                return;

            LayerData.EnableAllBlocks();
        }

        private void OnDisableAllBlocks()
        {
            if (LayerData == null)
                return;

            LayerData.DisableAllBlocks();
        }

        private void OnGoToPreviousBlock()
        {
            if (LayerData == null)
                return;

            LayerData.GoToPreviousBlock();
        }

        private void OnGoToNextBlock()
        {
            if (LayerData == null)
                return;

            LayerData.GoToNextBlock();
        }

        private void OnGenerateGuid()
        {
            //Clipboard.SetText(Guid.NewGuid().ToString("D"));
            Clipboard.SetDataObject(Guid.NewGuid().ToString("D"), false);
        }

        public void OnImportsSatisfied()
        {
            ReprocessCanvasRendererExtensions();
            ReprocessImporterExporterExtensions();
        }

        private void ReprocessImporterExporterExtensions()
        {
            layerDataImporterViewModels.Clear();
            foreach (var importer in layerDataImporters)
            {
                try
                {
                    importer.Initialize(importExportExtensionPath);
                }
                catch (Exception ex)
                {
                    var msg = new StringBuilder();
                    msg.AppendLine(string.Format("Importer '{0}' failed to initialize", importer.DisplayName));
                    msg.AppendLine();
                    msg.Append(ex.ToString());
                    MessageBox.Show(msg.ToString(), "Initialize Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    continue;
                }

                layerDataImporterViewModels.Add(new LayerDataImporterViewModel(this, importer));
            }
            HasLayerDataImporter = layerDataImporterViewModels.Count > 0;

            layerDataExporterViewModels.Clear();
            foreach (var exporter in layerDataExporters)
            {
                try
                {
                    exporter.Initialize(importExportExtensionPath);
                }
                catch (Exception ex)
                {
                    var msg = new StringBuilder();
                    msg.AppendLine(string.Format("Exporter '{0}' failed to initialize", exporter.DisplayName));
                    msg.AppendLine();
                    msg.Append(ex.ToString());
                    MessageBox.Show(msg.ToString(), "Initialize Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    continue;
                }

                layerDataExporterViewModels.Add(new LayerDataExporterViewModel(this, exporter));
            }
            HasLayerDataExporter = layerDataExporterViewModels.Count > 0;

            HasLayerDataImporterOrExporter = HasLayerDataImporter || HasLayerDataExporter;
        }

        private void ReprocessCanvasRendererExtensions()
        {
            foreach (var crvm in canvasBackgroundRenderers.Concat(canvasForegroundRenderers))
            {
                crvm.EnabledChanged -= canvasRendererViewModel_EnabledChanged;
                crvm.Dispose();
            }

            canvasForegroundRenderers.Clear();
            canvasBackgroundRenderers.Clear();

            var geometryHelper = new GeometryHelper();
            foreach (var cr in canvasRenderers)
            {
                cr.Initialize(this, geometryHelper, App.GlobalPropertyChangedNotifier);
                var crvm = new CanvasRendererViewModel(this, new CanvasRendererWrapper(cr));
                if (cr.RenderPlace == RenderPlace.Background)
                    canvasBackgroundRenderers.Add(crvm);
                else if (cr.RenderPlace == RenderPlace.Foreground)
                    canvasForegroundRenderers.Add(crvm);
            }

            LoadEnabledCanvasRenderers();

            foreach (var crvm in canvasBackgroundRenderers.Concat(canvasForegroundRenderers))
                crvm.EnabledChanged += canvasRendererViewModel_EnabledChanged;
        }

        private void canvasRendererViewModel_EnabledChanged(object sender, EventArgs e)
        {
            SaveEnabledCanvasRenderers();
        }

        private bool LoadEnabledCanvasRenderers()
        {
            try
            {
                var lines = File.ReadAllLines(Path.GetFullPath(EnabledCanvasRenderersFilename));

                foreach (var crw in canvasBackgroundRenderers.Concat(canvasForegroundRenderers))
                {
                    if (lines.Contains(crw.Name))
                        crw.IsEnabled = true;
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool SaveEnabledCanvasRenderers()
        {
            var retry = 5;

            while (retry-- >= 0)
            {
                try
                {
                    File.WriteAllLines(
                        Path.GetFullPath(EnabledCanvasRenderersFilename),
                        canvasBackgroundRenderers
                        .Concat(canvasForegroundRenderers)
                        .Where(x => x.IsEnabled)
                        .Select(x => x.Name));
                    return true;
                }
                catch (IOException)
                {
                }
            }

            return false;
        }

        private void RecomposeExtensions()
        {
            // init or recompose canvas renderers
            if (rootCatalog == null)
            {
                canvasRenderersDirectoryCatalog = new DirectoryCatalog(Path.Combine(extensionPath, "CanvasRenderers"), "*.dll");
                importExportDirectoryCatalog = new DirectoryCatalog(importExportExtensionPath, "*.dll");

                rootCatalog = new AggregateCatalog(canvasRenderersDirectoryCatalog, importExportDirectoryCatalog);
                ComposeExtensions(rootCatalog);
            }
            else
            {
                canvasRenderersDirectoryCatalog.Refresh();
                importExportDirectoryCatalog.Refresh();
            }
        }

        private AggregateCatalog rootCatalog;
        private DirectoryCatalog canvasRenderersDirectoryCatalog;
        private DirectoryCatalog importExportDirectoryCatalog;

        private void ComposeExtensions(AggregateCatalog catalog)
        {
            var container = new CompositionContainer(catalog);

            try
            {
                container.ComposeParts(this);
            }
            catch (Exception ex)
            {
                var msg = string.Format("A composition error occured:\r\n\r\n{0}", ex);
                MessageBox.Show(msg, "Composition Error", MessageBoxButton.OK, MessageBoxImage.Error);

                if (ex is ReflectionTypeLoadException)
                {
                    var rtlex = (ReflectionTypeLoadException)ex;
                    foreach (var subex in rtlex.LoaderExceptions)
                        MessageBox.Show(subex.ToString(), "Composition Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        IReadOnlyLayerDataViewModel IReadOnlyRootViewModel.LayerData
        {
            get { return LayerData; }
        }

        IReadOnlyComponentToolBoxViewModel IReadOnlyRootViewModel.ComponentToolBox
        {
            get { return ComponentToolBox; }
        }

        IReadOnlyFeatureToolBoxViewModel IReadOnlyRootViewModel.FeatureToolBox
        {
            get { return FeatureToolBox; }
        }

        IReadOnlyElementToolBoxViewModel IReadOnlyRootViewModel.ElementToolBox
        {
            get { return ElementToolBox; }
        }
    }

    public enum SaveStatus
    {
        None,
        Saving,
        SaveSucceeded,
        SaveFailed,
    }

    public abstract class RootedViewModel : ViewModelBase, IReadOnlyRootedViewModel
    {
        public RootViewModel Root { get; private set; }

        public RootedViewModel(RootViewModel root)
        {
            if (root == null)
                throw new ArgumentNullException("root");

            Root = root;
        }

        protected bool SetDataValue<T>(ref T field, T value, [CallerMemberName]string propertyName = null)
        {
            if (SetValue(ref field, value, propertyName))
            {
                Root.IsDataModified = true;
                return true;
            }
            return false;
        }

        protected override void OnPropertyChanged(string propertyName = null)
        {
            base.OnPropertyChanged(propertyName);
            if (propertyName != null)
                App.GlobalPropertyChangedNotifier.Raise(this, propertyName);
        }

        IReadOnlyRootViewModel IReadOnlyRootedViewModel.Root
        {
            get { return Root; }
        }
    }
}
