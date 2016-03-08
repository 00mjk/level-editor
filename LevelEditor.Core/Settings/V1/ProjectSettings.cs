using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace LevelEditor.Core.Settings.V1
{
    public class ProjectSettings : ProjectSettingsBase<ProjectSettings>
    {
        public int Version { get { return 1; } set { } }

        public GameBoardSettings GameBoard { get; set; }
        public GameBoardElementsSettings GameElements { get; set; }
        public ScreenshotsSettings Screenshots { get; set; }
        public BackupSettings Backup { get; set; }

        public static ProjectSettings GenerateDefault()
        {
            return new ProjectSettings
            {
                GameBoard = GameBoardSettings.GenerateDefault(),
                GameElements = GameBoardElementsSettings.GenerateDefault(),
                Screenshots = ScreenshotsSettings.GenerateDefault(),
                Backup = BackupSettings.GenerateDefault(),
            };
        }
    }
}
