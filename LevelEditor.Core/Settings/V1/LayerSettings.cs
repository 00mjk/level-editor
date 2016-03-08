using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelEditor.Core.Settings.V1
{
    public class LayerSettings
    {
        public byte Default { get; set; }
        public Layer[] Layers { get; set; }
    }

    public class Layer
    {
        public string Name { get; set; }
        public byte Number { get; set; }
        public string Description { get; set; }
    }
}
