using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace LevelEditor.Core.Settings.V1
{
    public class Property
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public string[] Values { get; set; }
    }

    public class ComponentSettings
    {
        public string ShortToolTipText { get; set; }

        public string Type { get; set; }
        // or
        public string Path { get; set; }
        public string SearchPattern { get; set; }

        public Property[] Properties { get; set; }
    }
}
