using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using LevelEditor.Extensibility;
using LevelEditor.ViewModels;

namespace LevelEditor.Features.Selection
{
    public class ProximitySelectionTool : SelectionToolBase
    {
        public double Distance { get; set; }
        public int MaximumBouncingLevel { get; set; }

        public ProximitySelectionTool()
        {
            Distance = 100.0;
            MaximumBouncingLevel = 8;
        }

        protected override void PerformSelection()
        {
            if (MouseDownItem == null)
                return;

            var ctrlDown = IsControlKeyDown();

            if (ctrlDown == false)
                ClearSelection();

            var group = FindGroup(ControlToUnit(LastPosition));

            var isSelected = true;
            if (ctrlDown)
            {
                var trueCount = group.Count(x => x.IsSelected == true);
                var falseCount = group.Count(x => x.IsSelected == false);

                if (trueCount == 0 || falseCount == 0)
                    isSelected = trueCount == 0;
                else
                    isSelected = trueCount >= falseCount;
            }

            foreach (var element in group)
                element.IsSelected = isSelected;
        }

        private ElementInstanceViewModel[] FindGroup(Point unitPosition)
        {
            var group = new HashSet<ElementInstanceViewModel>();
            FindGroupInternal(unitPosition, MaximumBouncingLevel, group);
            return group.ToArray();
        }

        private void FindGroupInternal(Point unitPosition, int level, HashSet<ElementInstanceViewModel> group)
        {
            if (level < 0)
                return;

            foreach (var element in Block.Instances)
            {
                var dx = (element.UnitPositionX - unitPosition.X) * Block.Info.BlockWidth;
                var dy = (element.UnitPositionY - unitPosition.Y) * Block.Info.BlockHeight;

                var distance = Math.Sqrt(dx * dx + dy * dy);

                if (distance <= Distance)
                {
                    if (group.Add(element))
                        FindGroupInternal(new Point(element.UnitPositionX, element.UnitPositionY), level - 1, group);
                }
            }
        }

        protected override ToolBoxToolRenderer GenerateRendererOverride(UIElement adornedElement)
        {
            return null;
        }
    }
}
