using Bitcraft.UI.Core;
using LevelEditor.Core.Settings.V1;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LevelEditor.ViewModels.ExportScreenshots
{
    public class ExportLocationViewModel : RootedViewModel
    {
        private string exportPath;
        public string ExportPath
        {
            get { return exportPath; }
            set
            {
                if (SetValue(ref exportPath, value))
                    ResolveToPath = EvaluatePath();
            }
        }

        private string resolveToPath;
        public string ResolveToPath
        {
            get { return resolveToPath; }
            private set { SetValue(ref resolveToPath, value); }
        }

        public enum ErrorLevel
        {
            Valid,
            DoesNotExist,
            Invalid
        }

        private ErrorLevel resolvedPathErrorLevel = ErrorLevel.Invalid;
        public ErrorLevel ResolvedPathErrorLevel
        {
            get { return resolvedPathErrorLevel; }
            private set { SetValue(ref resolvedPathErrorLevel, value); }
        }

        private static readonly string[] availablePathModes = new string[]
        {
            "Relative to level file",
            "Relative to Level Editor",
            "Relative to project settings file",
            "Absolute"
        };

        public string[] AvailablePathModes { get { return availablePathModes; } }

        private int selectedPathModeIndex;
        public int SelectedPathModeIndex
        {
            get { return selectedPathModeIndex; }
            set
            {
                if (SetValue(ref selectedPathModeIndex, value))
                    ResolveToPath = EvaluatePath();
            }
        }

        public bool HasPresets { get; private set; }

        public string[] AvailablePresets { get; private set; }

        private int selectedPresetIndex;
        public int SelectedPresetIndex
        {
            get { return selectedPresetIndex; }
            set
            {
                if (SetValue(ref selectedPresetIndex, value))
                {
                    if (selectedPresetIndex >= AvailablePresets.Length - 1)
                        ExportPath = string.Empty;
                    else
                        ExportPath = AvailablePresets[selectedPresetIndex];
                }
            }
        }

        public ICommand BrowseCommand { get; private set; }
        public ICommand OpenPathCommand { get; private set; }

        public ExportLocationViewModel(RootViewModel root)
            : base(root)
        {
            var s = root.Settings.Screenshots;

            if (s != null && s.Location != null)
            {
                if (s.Location.ExportPathPresets != null)
                {
                    var temp = s.Location.ExportPathPresets
                        .Where(x => string.IsNullOrWhiteSpace(x) == false)
                        .Concat(new[] { "Custom" })
                        .ToArray();

                    if (temp.Length > 1)
                    {
                        AvailablePresets = temp;
                        SelectedPresetIndex = AvailablePresets.Length - 1;
                        HasPresets = true;
                    }
                }

                ExportPath = s.Location.ExportPath;

                if (s.Location.PathMode >= 0 && s.Location.PathMode < AvailablePathModes.Length)
                    SelectedPathModeIndex = s.Location.PathMode;
            }

            ResolveToPath = EvaluatePath();

            BrowseCommand = new AnonymousCommand(OnBrowse);
            OpenPathCommand = new AnonymousCommand(OnOpenPath);
        }

        public void Initialize()
        {
            ResolveToPath = EvaluatePath();
        }

        private void OnBrowse()
        {
        }

        private void OnOpenPath()
        {
            if (ResolvedPathErrorLevel == ErrorLevel.Valid)
                Process.Start(ResolveToPath);
        }

        private string EvaluatePath()
        {
            string basePath = "";

            if (SelectedPathModeIndex == 0) // Layer Data file path
            {
                basePath = Path.GetDirectoryName(Root.LayerData.FullFilename);
            }
            else if (SelectedPathModeIndex == 1) // Level Editor executable path
            {
                basePath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            }
            else if (SelectedPathModeIndex == 2) // Project settings file path
            {
                basePath = Root.Settings.SettingsFilePath;
            }

            basePath = basePath ?? string.Empty;
            ExportPath = ExportPath ?? string.Empty;

            try
            {
                if (Path.IsPathRooted(basePath) && Path.IsPathRooted(ExportPath))
                {
                    ResolvedPathErrorLevel = ErrorLevel.Invalid;
                    return null;
                }

                var path = Path.Combine(basePath, ExportPath);

                if (Path.IsPathRooted(path) == false)
                {
                    ResolvedPathErrorLevel = ErrorLevel.Invalid;
                    return path;
                }

                path = Path.GetFullPath(path);

                if (Directory.Exists(path) == false)
                    ResolvedPathErrorLevel = ErrorLevel.DoesNotExist;
                else
                    ResolvedPathErrorLevel = ErrorLevel.Valid;

                return path;
            }
            catch
            {
                ResolvedPathErrorLevel = ErrorLevel.Invalid;
                return null;
            }
        }
    }
}
