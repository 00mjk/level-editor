using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Bitcraft.UI.Core;
using LevelEditor.Extensibility;
using LevelEditor.Extensibility.ReadOnlyViewModels;
using LevelEditor.Features.Selection;

namespace LevelEditor.ViewModels
{
    public class ToolBoxTool : IReadOnlyToolBoxTool
    {
        public ToolBoxTool(string name, ToolCategory category, IMouseTool tool)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Invalid 'name' argument.", "name");
            if (tool == null)
                throw new ArgumentNullException("tool");

            Id = Guid.NewGuid();
            Name = name;
            Category = category;
            Tool = tool;
        }

        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public ToolCategory Category { get; private set; }
        public IMouseTool Tool { get; private set; }
    }

    public class FeatureToolBoxViewModel : RootedViewModel, IReadOnlyFeatureToolBoxViewModel
    {
        public FeatureToolBoxViewModel(RootViewModel root)
            : base(root)
        {
            AvailableTools = new[]
            {
                new ToolBoxTool("Standard [Selection]", ToolCategory.Selection, new StandardSelectionTool()),
                new ToolBoxTool("Proximity [Selection]", ToolCategory.Selection, new ProximitySelectionTool()),
            };

            SelectedTool = AvailableTools[0];

            KeyCommand = new AnonymousCommand<Key>(OnKey);
        }

        public ICommand KeyCommand { get; private set; }

        private void OnKey(Key key)
        {
            switch (key)
            {
                case Key.S:
                    ToggleTool(ToolCategory.Selection);
                    break;
            }
        }

        private void ToggleTool(ToolCategory category)
        {
            if (SelectedTool == null || SelectedTool.Category != category)
                SelectedTool = AvailableTools.First(x => x.Category == category);
            else
            {
                var categorizedTools = AvailableTools
                    .Where(x => x.Category == category)
                    .ToArray();
                var currentIndex = Array.FindIndex(AvailableTools, x => x.Id == SelectedTool.Id);

                currentIndex++;
                if (currentIndex >= categorizedTools.Length)
                    currentIndex = 0;

                SelectedTool = categorizedTools[currentIndex];
            }
        }

        public ToolBoxTool[] AvailableTools { get; private set; }

        private ToolBoxTool selectedTool;
        public ToolBoxTool SelectedTool
        {
            get { return selectedTool; }
            set
            {
                if (SetValue(ref selectedTool, value))
                {
                    var tool = selectedTool.Tool;
                    if (tool != null && Root.LayerData != null)
                        Root.LayerData.SelectionTool = tool;
                }
            }
        }

        IReadOnlyToolBoxTool[] IReadOnlyFeatureToolBoxViewModel.AvailableTools
        {
            get { return AvailableTools; }
        }

        IReadOnlyToolBoxTool IReadOnlyFeatureToolBoxViewModel.SelectedTool
        {
            get { return SelectedTool; }
        }
    }
}
