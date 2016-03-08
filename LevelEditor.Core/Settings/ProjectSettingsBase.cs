using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace LevelEditor.Core.Settings
{
    public abstract class ProjectSettingsBase<T> where T : ProjectSettingsBase<T>
    {
        [XmlIgnore]
        public string SettingsFilePath { get; private set; }

        public void Save(string filename)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (var fs = new FileStream(filename, FileMode.Create, FileAccess.Write, FileShare.Write))
                serializer.Serialize(fs, this);
        }

        public static T Load(string filename)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using (var fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                var settings = (T)serializer.Deserialize(fs);
                settings.SettingsFilePath = Path.GetDirectoryName(filename);
                return settings;
            }
        }
    }
}
