using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Bitcraft.UI.Core;
using LayerDataReaderWriter.V9;
using LevelEditor.Behaviors;
using LevelEditor.Extensibility;
using LevelEditor.Extensibility.ReadOnlyViewModels;
using LevelEditor.Features.Selection;
using Bitcraft.Core.Extensions;
using Bitcraft.UI.Core.Extensions;
using LevelEditor.Core;

namespace LevelEditor.ViewModels
{
    public class LayerDataViewModel : RootedViewModel, IDisposable, IReadOnlyLayerDataViewModel
    {
        public string FullFilename { get; private set; }

        public bool IsSnappingToGrid { get; set; }

        private bool workInProgress;
        public bool WorkInProgress
        {
            get { return workInProgress; }
            set { SetValue(ref workInProgress, value); }
        }

        private string workInProgressText;
        public string WorkInProgressText
        {
            get { return workInProgressText; }
            set { SetValue(ref workInProgressText, value); }
        }

        private double workInProgressRate;
        public double WorkInProgressRate
        {
            get { return workInProgressRate; }
            set { SetValue(ref workInProgressRate, value); }
        }

        private LayerBlockViewModel selectedBlock;
        public LayerBlockViewModel SelectedBlock
        {
            get { return selectedBlock; }
            set
            {
                if (SetValue(ref selectedBlock, value))
                {
                    if (SelectedBlock != null)
                        SelectionTool.SetBlock(SelectedBlock);
                }
            }
        }

        private IMouseTool selectionTool;
        public IMouseTool SelectionTool
        {
            get { return selectionTool; }
            internal set
            {
                if (SetValue(ref selectionTool, value))
                {
                    if (SelectedBlock != null)
                        SelectionTool.SetBlock(SelectedBlock);
                }
            }
        }

        public ICommand FuckTheOneWayToSourceCommand { get; private set; }
        private ICommand requestRenderCommand;
        private ICommand requestScreenshotCommand;

        private void OnFuckTheOneWayToSource(ICommand[] commands)
        {
            requestRenderCommand = commands[0];
            requestScreenshotCommand = commands[1];
        }

        public void RequestRender()
        {
            requestRenderCommand.ExecuteIfPossible();
        }

        public System.Windows.Media.Imaging.BitmapSource RequestScreenshot()
        {
            var dic = new Dictionary<string, System.Windows.Media.Imaging.BitmapSource>();
            requestScreenshotCommand.ExecuteIfPossible(dic);

            System.Windows.Media.Imaging.BitmapSource result = null;
            dic.TryGetValue("shot", out result);

            return result;
        }

        public IEnumerable<CanvasRendererWrapper> CanvasBackgroundRendererWrappers
        {
            get
            {
                return Root.CanvasBackgroundRenderers.Select(x => x.Wrapper);
            }
        }

        public IEnumerable<CanvasRendererWrapper> CanvasForegroundRendererWrappers
        {
            get
            {
                return Root.CanvasForegroundRenderers.Select(x => x.Wrapper);
            }
        }

        private ObservableCollection<LayerBlockViewModel> blocks = new ObservableCollection<LayerBlockViewModel>();

        private IReadOnlyObservableCollection<LayerBlockViewModel> _blocks;
        public IReadOnlyObservableCollection<LayerBlockViewModel> Blocks
        {
            get { return _blocks; }
            private set { SetValue(ref _blocks, value); }
        }

        public LayerDataViewModel(RootViewModel root)
            : base(root)
        {
            FuckTheOneWayToSourceCommand = new AnonymousCommand<ICommand[]>(OnFuckTheOneWayToSource);

            Blocks = new Bitcraft.UI.Core.ReadOnlyObservableCollection<LayerBlockViewModel>(blocks);
            Blocks.CollectionChanged += Blocks_CollectionChanged;

            IsSnappingToGrid = true;

            AddBlockCommand = new AnonymousCommand(OnAddBlock);
            DuplicateBlockCommand = new AnonymousCommand(OnDuplicateBlock) { IsEnabled = false };

            if (Blocks.Count > 0)
                SelectedBlock = Blocks[0];
        }

        private void Blocks_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Root.IsDataModified = true;
            ((AnonymousCommand)DuplicateBlockCommand).IsEnabled = Blocks.Count > 0;
        }

        public ICommand AddBlockCommand { get; private set; }
        public ICommand DuplicateBlockCommand { get; private set; }

        private void OnAddBlock()
        {
            LayerBlockViewModel newBlock = null;

            if (Blocks.Count == 0)
                newBlock = new LayerBlockViewModel(Root, this);
            else
            {
                var baseBlock = SelectedBlock ?? Blocks[Blocks.Count - 1];
                newBlock = DuplicateBlock(baseBlock, false);
            }

            AddNewBlock(newBlock);
        }

        private void OnDuplicateBlock()
        {
            if (SelectedBlock == null)
                throw new InvalidOperationException("No selected block");

            AddNewBlock(DuplicateBlock(SelectedBlock, true));
        }

        public void AddNewBlock(LayerBlockViewModel newBlock)
        {
            if (newBlock == null)
                throw new ArgumentNullException("newBlock");

            blocks.Add(newBlock);

            if (Blocks.Count == 1)
                SelectedBlock = newBlock;
            else if (Root.Settings.GameBoard.SelectNewlyCreatedBlock)
                SelectedBlock = Blocks[Blocks.Count - 1];

            SelectionTool = Root.FeatureToolBox.SelectedTool.Tool;
        }

        private LayerBlockViewModel DuplicateBlock(LayerBlockViewModel baseBlock, bool duplicateElements)
        {
            var newBlock = new LayerBlockViewModel(Root, this);

            newBlock.Difficulty = baseBlock.Difficulty;

            newBlock.Info.BlockWidth = baseBlock.Info.BlockWidth;
            newBlock.Info.BlockHeight = baseBlock.Info.BlockHeight;
            newBlock.Info.SnapX = baseBlock.Info.SnapX;
            newBlock.Info.SnapY = baseBlock.Info.SnapY;
            newBlock.Info.Zoom = baseBlock.Info.Zoom;

            if (duplicateElements)
            {
                foreach (var sourceElement in baseBlock.Instances)
                {
                    newBlock.AddInstance(
                        new ElementInstanceViewModel(sourceElement.UniqueIdentifier, Root, newBlock, sourceElement.ProduceLayerBlockElement())
                        {
                            IsSelected = sourceElement.IsSelected,
                        });
                }
            }

            return newBlock;
        }

        public bool MoveBlockUp(LayerBlockViewModel block)
        {
            if (block == null)
                return false;

            var index = blocks.IndexOf(block);
            if (index <= 0)
                return false;

            var selected = SelectedBlock;

            blocks.RemoveAt(index);
            blocks.Insert(index - 1, block);

            SelectedBlock = selected;

            return true;
        }

        public bool MoveBlockDown(LayerBlockViewModel block)
        {
            if (block == null)
                return false;

            var index = blocks.IndexOf(block);
            if (index == -1 || index == blocks.Count - 1)
                return false;

            var selected = SelectedBlock;

            blocks.RemoveAt(index);
            blocks.Insert(index + 1, block);

            SelectedBlock = selected;

            return true;
        }

        public void EnableAllBlocks()
        {
            foreach (var block in Blocks)
                block.IsEnabled = true;
        }

        public void DisableAllBlocks()
        {
            foreach (var block in Blocks)
                block.IsEnabled = false;
        }

        public void GoToPreviousBlock()
        {
            if (blocks.Count == 0)
                return;

            var index = blocks.IndexOf(SelectedBlock);

            if (index < 0)
                SelectedBlock = blocks[0];
            else
            {
                index--;
                if (index < 0)
                    index = 0;
                SelectedBlock = blocks[index];
            }
        }

        public void GoToNextBlock()
        {
            if (blocks.Count == 0)
                return;

            var index = blocks.IndexOf(SelectedBlock);

            if (index < 0)
                SelectedBlock = blocks[0];
            else
            {
                index++;
                if (index >= blocks.Count)
                    index = blocks.Count - 1;
                SelectedBlock = blocks[index];
            }
        }

        public void LoadData(string fullFilename, LayerFile layerFile)
        {
            FullFilename = fullFilename;

            Clear();

            foreach (var block in layerFile.Blocks)
            {
                var newBlock = new LayerBlockViewModel(Root, this, block);
                blocks.Add(newBlock);
            }

            if (Blocks.Count > 0)
            {
                var firstEnabledBlock = Blocks.FirstOrDefault(b => b.IsEnabled);
                if (firstEnabledBlock != null)
                    SelectedBlock = firstEnabledBlock;
                else
                    SelectedBlock = Blocks[0];
            }
        }

        public LayerFile SaveData()
        {
            var layerBlocks = Blocks
                .Select(x => x.ProduceLayerBlock())
                .OrderBy(x => x.Difficulty)
                .ToArray();

            return new LayerFile(null, layerBlocks);
        }

        public void Clear()
        {
            foreach (var block in Blocks)
            {
                block.Clear();
                block.Dispose();
            }
            blocks.Clear();
        }

        public void Dispose()
        {
            Blocks.CollectionChanged -= Blocks_CollectionChanged;
        }

        public void RemoveBlock(LayerBlockViewModel layerBlock)
        {
            blocks.Remove(layerBlock);
        }

        IReadOnlyLayerBlockViewModel IReadOnlyLayerDataViewModel.SelectedBlock
        {
            get { return SelectedBlock; }
        }

        IReadOnlyObservableCollection<IReadOnlyLayerBlockViewModel> IReadOnlyLayerDataViewModel.Blocks
        {
            get { return Blocks; }
        }

        bool IReadOnlyLayerDataViewModel.MoveBlockUp(IReadOnlyLayerBlockViewModel block)
        {
            return MoveBlockUp(block as LayerBlockViewModel);
        }

        bool IReadOnlyLayerDataViewModel.MoveBlockDown(IReadOnlyLayerBlockViewModel block)
        {
            return MoveBlockDown(block as LayerBlockViewModel);
        }
    }
}
