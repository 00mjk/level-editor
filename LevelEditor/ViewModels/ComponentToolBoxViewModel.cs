using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LevelEditor.Extensibility.ReadOnlyViewModels;
using Bitcraft.Core.Extensions;
using Bitcraft.Core.Utility;

namespace LevelEditor.ViewModels
{
    public class ComponentToolBoxViewModel : RootedViewModel, IReadOnlyComponentToolBoxViewModel
    {
        private ComponentViewModel[] components;
        public ComponentViewModel[] Components
        {
            get { return components; }
            private set { SetValue(ref components, value); }
        }

        public ComponentToolBoxViewModel(RootViewModel root)
            : base(root)
        {
            var componentList = new List<ComponentViewModel>();

            foreach (var c in root.Settings.GameBoard.Components.EmptyIfNull())
            {
                if (string.IsNullOrWhiteSpace(c.Type) == false)
                    componentList.Add(new ComponentViewModel(Root, c));
                else if (string.IsNullOrWhiteSpace(c.Path) == false && string.IsNullOrWhiteSpace(c.SearchPattern) == false)
                    componentList.AddRange(LoadComponentsFromPath(c.Path, c.SearchPattern));
            }

            components = componentList.OrderBy(x => x.Type).ToArray();
        }

        private IEnumerable<ComponentViewModel> LoadComponentsFromPath(string path, string searchPattern)
        {
            if (Path.IsPathRooted(path) == false)
                path = Path.Combine(Root.Settings.SettingsFilePath, path);

            path = Path.GetFullPath(path);

            if (Directory.Exists(path) == false)
                yield break;

            foreach (var file in Directory.GetFiles(path, searchPattern, SearchOption.TopDirectoryOnly))
            {
                // for the shortToolTipText parameter, could automatically look for the n first letters of the component filenames that produce unique names
                yield return new ComponentViewModel(Root, Path.GetFileNameWithoutExtension(file));
            }
        }

        public ComponentViewModel GetComponentFromType(string type)
        {
            return Components.FirstOrDefault(c => string.Equals(c.Type, type, StringComparison.Ordinal));
        }

        IReadOnlyComponentViewModel[] IReadOnlyComponentToolBoxViewModel.Components
        {
            get { return Components; }
        }

        IReadOnlyComponentViewModel IReadOnlyComponentToolBoxViewModel.GetComponentFromType(string type)
        {
            return GetComponentFromType(type);
        }

        private string filter;
        public string Filter
        {
            get { return filter; }
            set
            {
                if (filter != value)
                {
                    if (string.IsNullOrWhiteSpace(value))
                    {
                        foreach (var c in components)
                            c.IsVisible = true;
                    }
                    else
                    {
                        foreach (var c in components)
                            c.IsVisible = PascalCasing.IsMatching(c.Type, value, StringComparison.OrdinalIgnoreCase);
                    }
                    filter = value;
                }
            }
        }
    }
}
