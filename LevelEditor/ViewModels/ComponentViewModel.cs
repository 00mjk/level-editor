using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Bitcraft.UI.Core;
using LevelEditor.Behaviors;
using LevelEditor.Extensibility.ReadOnlyViewModels;
using LevelEditor.Core.Settings.V1;

namespace LevelEditor.ViewModels
{
    public class ComponentViewModel : RootedViewModel, IDragSource, IReadOnlyComponentViewModel
    {
        public string Type { get; private set; }
        public ComponentSettings Settings { get; private set; }

        public ComponentViewModel(RootViewModel root, string type)
            : base(root)
        {
            Type = type;
        }

        public ComponentViewModel(RootViewModel root, ComponentSettings componentSettings)
            : base(root)
        {
            Type = componentSettings.Type;
            Settings = componentSettings;
        }

        private bool isVisible = true;
        public bool IsVisible
        {
            get { return isVisible; }
            set { SetValue(ref isVisible, value); }
        }

        public DragDropEffects BeginDrag(Point position)
        {
            return DragDropEffects.Copy;
        }
    }
}
