using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using LayerDataReaderWriter;
using LevelEditor.Core.Settings;
using LevelEditor.ViewModels;

namespace LevelEditor
{
    public class BackupManager
    {
        private const string BackupFileExtension = ".bytes";

        private Regex backupFileRegex = new Regex(@"^[0-9a-fA-F]{32}_\d{4}\.\d{2}\.\d{2}_\d{2}\.\d{2}\.\d{2}$", RegexOptions.Compiled);

        private Timer timer;
        private RootViewModel root;

        private string backupsPath;
        private bool hasChangedSinceLastCheck;
        private int failedCount;

        private readonly string guid = Guid.NewGuid().ToString("N");

        public BackupManager(RootViewModel root)
        {
            if (root == null)
                throw new ArgumentNullException("root");

            this.root = root;

            SetupBackupPath();

            CleanupBackups(true);

            var interval = root.Settings.Backup.AutoBackupInterval * 1000;
            timer = new Timer(_ => OnTimer(), null, interval, interval);
        }

        private void SetupBackupPath()
        {
            backupsPath = root.Settings.Backup.BackupsPath;

            if (Path.IsPathRooted(backupsPath) == false)
            {
                var appPath = AppDomain.CurrentDomain.BaseDirectory;
                backupsPath = Path.Combine(appPath, backupsPath);
            }

            backupsPath = Path.GetFullPath(backupsPath);

            if (Directory.Exists(backupsPath) == false)
                Directory.CreateDirectory(backupsPath);
        }

        public void OnDataChanged()
        {
            hasChangedSinceLastCheck = true;
        }

        private void OnTimer()
        {
            if (hasChangedSinceLastCheck)
            {
                // suspend the timer
                timer.Change(Timeout.Infinite, Timeout.Infinite);

                if (PerformBackup() == false)
                    failedCount++;
                else
                    failedCount = 0;

                if (failedCount >= 3)
                {
                    timer.Change(Timeout.Infinite, Timeout.Infinite);
                    timer = null;
                    return; // TODO: log an error to user
                }

                // resume the timer
                var interval = root.Settings.Backup.AutoBackupInterval * 1000;
                timer.Change(interval, interval);
            }

            hasChangedSinceLastCheck = false;
        }

        private bool CleanupBackups(bool deleteAll)
        {
            bool result = true;

            try
            {
                IEnumerable<string> filesToDelete;

                if (deleteAll)
                {
                    filesToDelete = Directory.GetFiles(backupsPath, string.Format("*{0}", BackupFileExtension), SearchOption.TopDirectoryOnly)
                        .Where(IsValidBackupFilename);
                }
                else
                {
                    filesToDelete = Directory.GetFiles(backupsPath, string.Format("{0}_*{1}", guid, BackupFileExtension), SearchOption.TopDirectoryOnly)
                        .Where(IsValidBackupFilename)
                        .OrderByDescending(f => f)
                        .Skip(root.Settings.Backup.NumberOfBackupsToKeep);
                }

                foreach (var file in filesToDelete)
                {
                    try
                    {
                        File.Delete(file);
                    }
                    catch // swallow error silently
                    {
                        result = false;
                    }
                }
            }
            catch
            {
                result = false;
            }

            return result;
        }

        private bool PerformBackup()
        {
            try
            {
                var filePath = Path.Combine(backupsPath, string.Format("{0}_{1:yyyy.MM.dd_HH.mm.ss}{2}", guid, DateTime.Now, BackupFileExtension));
                using (var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.Write))
                    ReaderWriterManager.Write(root.LayerData.SaveData(), fs, Encoding.UTF8, App.CurrentFileFormatVersion);

                return CleanupBackups(false);
            }
            catch
            {
                return false;
            }
        }

        private bool IsValidBackupFilename(string file)
        {
            file = Path.GetFileNameWithoutExtension(file);

            if (backupFileRegex.IsMatch(file) == false)
                return false;

            return true;
        }
    }
}
