using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bitcraft.UI.Core;
using LevelEditor.Extensibility.ReadOnlyViewModels;

namespace LevelEditor.ViewModels
{
    public class LayerBlockInfoViewModel : RootedViewModel, IReadOnlyLayerBlockInfoViewModel
    {
        private LayerBlockViewModel parent;

        public LayerBlockInfoViewModel(LayerBlockViewModel parent, double blockWidth, double blockHeight)
            : this(parent, blockWidth, blockHeight, null)
        {
        }

        public LayerBlockInfoViewModel(LayerBlockViewModel parent, double blockWidth, double blockHeight, bool[] userFlags)
            : base(parent.Root)
        {
            this.parent = parent;

            BlockWidth = blockWidth;
            BlockHeight = blockHeight;

            if (Root.Settings.GameBoard.FlagSemantics != null)
            {
                flags = new ObservableCollection<FlagViewModel>(Root.Settings.GameBoard.FlagSemantics
                    .Select(fs => new FlagViewModel(Root, fs.FlagNumber, fs.Text, GetFlag(userFlags, fs.FlagNumber))));
            }
            else
                flags = new ObservableCollection<FlagViewModel>();

            Flags = new Bitcraft.UI.Core.ReadOnlyObservableCollection<FlagViewModel>(flags);
        }

        private bool GetFlag(bool[] userFlags, int index)
        {
            if (userFlags == null || index >= userFlags.Length)
                return false;
            return userFlags[index];
        }

        private bool isCanvasSizeMode = true;
        public bool IsCanvasSizeMode
        {
            get { return isCanvasSizeMode; }
            set { SetValue(ref isCanvasSizeMode, value); }
        }

        private double zoom = 1.0;
        public double Zoom
        {
            get { return zoom; }
            set { SetValue(ref zoom, value); }
        }

        private double blockWidth;
        public double BlockWidth
        {
            get { return blockWidth; }
            set
            {
                var prevWidth = blockWidth;
                if (SetDataValue(ref blockWidth, value))
                {
                    if (IsCanvasSizeMode)
                        CanvasResizeItems(prevWidth, blockWidth, blockHeight, blockHeight);
                }
            }
        }

        private double blockHeight;
        public double BlockHeight
        {
            get { return blockHeight; }
            set
            {
                var prevHeight = blockHeight;
                if (SetDataValue(ref blockHeight, value))
                {
                    if (IsCanvasSizeMode)
                        CanvasResizeItems(blockWidth, blockWidth, prevHeight, blockHeight);
                }
            }
        }

        private double snapX;
        public double SnapX
        {
            get { return snapX; }
            set { SetValue(ref snapX, value); }
        }

        private double snapY;
        public double SnapY
        {
            get { return snapY; }
            set { SetValue(ref snapY, value); }
        }

        private ObservableCollection<FlagViewModel> flags = new ObservableCollection<FlagViewModel>();
        public IReadOnlyObservableCollection<FlagViewModel> Flags { get; private set; }

        private void CanvasResizeItems(double prevWidth, double newWidth, double prevHeight, double newHeight)
        {
            if (parent.Instances == null)
                return;

            var offsetX = (newWidth - prevWidth) / 2.0;
            var offsetY = (newHeight - prevHeight) / 2.0;

            var toDelete = new List<ElementInstanceViewModel>();

            foreach (var element in parent.Instances)
            {
                var px = element.UnitPositionX * prevWidth;
                var py = element.UnitPositionY * prevHeight;

                var newPx = (px + offsetX) / newWidth;
                var newPy = (py + offsetY) / newHeight;

                if (newPx < 0.0 || newPx > 1.0 || newPy < 0.0 || newPy > 1.0)
                {
                    toDelete.Add(element);
                    continue;
                }

                element.UnitPositionX = newPx;
                element.UnitPositionY = newPy;
            }

            foreach (var element in toDelete)
            {
                element.Dispose();
                parent.RemoveInstance(element);
            }

            BlockWidth = newWidth;
            BlockHeight = newHeight;
        }

        IReadOnlyObservableCollection<IReadOnlyFlagViewModel> IReadOnlyLayerBlockInfoViewModel.Flags
        {
            get { return Flags; }
        }
    }
}
