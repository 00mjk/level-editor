using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelEditor.Core.Settings.V1
{
    public class BackupSettings
    {
        /// <summary>
        /// Interval of auto backup checks, in seconds.
        /// </summary>
        public int AutoBackupInterval { get; set; }
        public int NumberOfBackupsToKeep { get; set; }
        public string BackupsPath { get; set; }

        public static BackupSettings GenerateDefault()
        {
            return new BackupSettings
            {
                BackupsPath = "backups",
                AutoBackupInterval = 2 * 60,
                NumberOfBackupsToKeep = 15,
            };
        }
    }
}
