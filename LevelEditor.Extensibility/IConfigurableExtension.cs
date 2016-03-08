using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LevelEditor.Extensibility
{
    public interface IConfigurableExtension
    {
        bool HasConfiguration { get; }
        void Configure();
    }
}
